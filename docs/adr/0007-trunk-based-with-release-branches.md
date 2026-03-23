# 0007. Trunk based with release branches

- Status: Accepted
- Date: 2026-01-14
- Deciders: Alex Morgan, Grace Underwood, Hannah Choi

## Context

We ran GitFlow for the first year. `develop` and `main` diverged, every release needed
a merge back that nobody wanted to do, and two hotfixes were lost because they went to
`main` and never reached `develop`.

## Options considered

### Keep GitFlow

Familiar. The back merge problem is a discipline problem, in theory. In practice we
lost two fixes in twelve months.

### Pure trunk based, deploy from `main`

Simplest. Needs feature flags for anything that takes more than a day, and we have
no flag infrastructure.

### Trunk based with short lived release branches

`main` is always deployable. Cut `release/x.y` when stabilising, fix on the branch,
cherry pick back to `main` in the same pull request.

## Decision

Trunk based with release branches. `develop` is deleted.

## Consequences

- One long lived branch. No back merge ritual.
- A hotfix must land on `main` too. The release pull request template has a checkbox,
  and a workflow comments if the cherry pick is missing after 24 hours.
- Release branches live for days, not weeks. If one lives longer, that is the signal
  that the release is too big.
