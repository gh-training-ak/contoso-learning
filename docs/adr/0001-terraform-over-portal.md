# 0001. Terraform owns the infrastructure

- Status: Accepted
- Date: 2025-01-21
- Deciders: Alex Morgan, Hannah Choi

## Context

The three environments drifted. Dev had a B1 plan with Always On off, test had a P1v3
somebody had scaled up during a load test and never scaled back, and prod had an app
setting that existed nowhere else. A bug that only reproduced in test cost two days
before somebody noticed the TLS minimum version differed.

Nobody could answer "what is actually deployed" without opening the portal.

## Options considered

### Portal plus a written runbook

Cheapest to start. Depends on people following it. We already proved we do not.

### Bicep

First party, good Azure coverage, no state file to manage. The team has no Bicep
experience and we also manage a small amount of non Azure infrastructure.

### Terraform

One language for everything, a plan we can read in a pull request, explicit state.
Costs us a state backend and the discipline to never touch the portal again.

## Decision

Terraform. One root module, environment differences expressed in `locals.sizing` and
a `.tfvars` file per environment. State in an Azure storage account with Entra auth.

We keep a Bicep translation under `infra/bicep` so the team can compare the two, but
it is not deployed.

## Consequences

- Every environment change is reviewable before it happens.
- `terraform plan` in a pull request shows drift, which is how we find portal edits.
- Anyone who clicks in the portal gets their change reverted on the next apply.
- We now own a state file. Losing it is an incident. It is backed up and versioned.
