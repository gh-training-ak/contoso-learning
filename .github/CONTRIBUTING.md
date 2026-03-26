# Contributing

## Before you start

Open an issue, or claim an existing one, so two people do not build the same thing.
Anything larger than a day needs a short design note on the issue first.

## Branches

`main` is protected. Branch from it and use the prefix that matches the work:

| Prefix | Use |
| --- | --- |
| `feature/` | New behaviour |
| `fix/` | Bug fix on `main` |
| `hotfix/` | Urgent fix that ships outside the train |
| `chore/` | Dependencies, tooling, no behaviour change |
| `docs/` | Documentation only |
| `release/` | Release stabilisation, cut by the release manager |

Branch names are lowercase with hyphens. Include the issue number when there is one:
`feature/412-mentor-availability`.

## Commits

Conventional commits. The release notes are generated from them.

```
feat(api): return paged mentor results
fix(domain): reject reviews from students with no completed match
chore(deps): bump xunit to 2.9.2
docs(adr): record the move to fixed window rate limiting
```

Add a `Co-authored-by:` trailer when you pair.

## Pull requests

- Keep them under roughly 400 changed lines. Split anything larger.
- Fill in the template. "Refactor" with no explanation gets sent back.
- CI must be green. Do not merge a red build and promise a follow up.
- One approval from a code owner is required, two for anything under `infra/`.

## Which merge button

| Situation | Button |
| --- | --- |
| Feature branch with a clean, meaningful history | Merge commit |
| Branch with "wip", "fix typo", "try again" commits | Squash and merge |
| Small change on a branch that is behind `main` | Rebase and merge |

`main` does not require linear history, so all three are available. Pick deliberately.
Do not squash a long lived branch that others have branched from, you will hand them
a conflict on every later merge.

## Tests

Every behaviour change needs a test that fails without it. Run the suite locally:

```bash
dotnet test Contoso.sln
cd web && npm test
```

## Database changes

Migrations are forward only. Number the file, never edit one that has shipped.
Additive changes only while the previous release is still running.
