# Contributing

## Before you start

Open an issue, or claim an existing one, so two people do not build the same thing.

## Branches

We run GitFlow. `main` holds what is in production, `develop` holds what is next.
Branch from `develop` and use the prefix that matches the work:

| Prefix | Branch from | Merge into |
| --- | --- | --- |
| `feature/` | `develop` | `develop` |
| `fix/` | `develop` | `develop` |
| `hotfix/` | `main` | `main`, then back merge to `develop` |
| `chore/` | `develop` | `develop` |
| `docs/` | `develop` | `develop` |

Branch names are lowercase with hyphens. Include the issue number when there is one:
`feature/412-mentor-availability`.

The back merge after a hotfix is not optional. If you skip it the fix disappears at the
next release.

## Commits

Conventional commits. The release notes are generated from them.

```
feat(api): return paged mentor results
fix(domain): reject reviews from students with no completed match
chore(deps): bump xunit to 2.9.2
```

Add a `Co-authored-by:` trailer when you pair.

## Pull requests

- Keep them under roughly 400 changed lines. Split anything larger.
- Fill in the template. "Refactor" with no explanation gets sent back.
- CI must be green. Do not merge a red build and promise a follow up.
- One approval from a code owner is required.

## Tests

Every behaviour change needs a test that fails without it. Run the suite locally:

```bash
dotnet test Contoso.sln
cd web && npm test
```

## Database changes

Migrations are forward only. Number the file, never edit one that has shipped.
