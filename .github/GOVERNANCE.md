# Governance

## Roles

| Role | Who | What they decide |
| --- | --- | --- |
| Maintainers | `@gh-training-ak/platform-team` | Architecture, release content, who else becomes a maintainer |
| Area owners | The teams in `CODEOWNERS` | Changes inside their directory |
| Contributors | Everyone with write access | Day to day changes through pull request |

## How decisions are made

Small changes: one code owner approval and a green build.

Anything that changes a public contract, adds a dependency, or changes the deployment
topology needs an ADR in `docs/adr`. Open it as a pull request, leave it open for two
working days, then merge it if nobody blocks. A block has to come with an alternative.

## Releases

A release manager rotates every month. They cut `release/x.y`, run the regression pass,
tag, and write the release notes. The tag triggers the release workflow.

## Adding a maintainer

Any maintainer can nominate. Needs agreement from the rest and no objections within a week.
