# Local setup

## What you need

| Tool | Version | Check |
| --- | --- | --- |
| .NET SDK | 8.0.4xx | `dotnet --version` |
| Node | 20 or later | `node --version` |
| Docker Desktop | any recent | `docker version` |
| Terraform | 1.9.x | `terraform version` |
| GitHub CLI | 2.x | `gh --version` |

`global.json` pins the SDK feature band. If `dotnet --version` reports something older
the build will tell you rather than quietly using the wrong compiler.

## First run

```bash
gh repo clone gh-training-ak/contoso-learning
cd contoso-learning
dotnet restore Contoso.sln
dotnet test Contoso.sln
```

54 tests should pass in about a second. If they do not, stop and fix that before
changing anything.

## Running the API

```bash
dotnet run --project src/Contoso.Api
```

| Endpoint | Purpose |
| --- | --- |
| `GET /healthz` | Liveness, no dependencies checked |
| `GET /api/mentors` | Search, takes the filter query string |
| `GET /api/mentors/{id}` | One mentor |
| `GET /api/mentors/subjects` | The subject enum, for the filter dropdown |
| `POST /api/match-requests` | Create a request |
| `PATCH /api/match-requests/{id}` | Accept or decline, reason required on a decline |

There is no Swagger UI. `docs/api.md` is the contract.

Repositories are in memory and start empty, so a search returns zero results until you
post something. That is deliberate: a seeded repository hides ordering bugs.
