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

## Running everything

```bash
export SQL_PASSWORD='Choose-A-Strong-One-1'      # PowerShell: $env:SQL_PASSWORD='...'
docker compose -f deploy/docker/docker-compose.yml up --build
```

Compose refuses to start if `SQL_PASSWORD` is unset. That is deliberate, there is no
default password anywhere in this repository.

## Applying the schema

```bash
for f in database/migrations/*.sql; do
  sqlcmd -S localhost,1433 -U sa -P "$SQL_PASSWORD" -C -d Contoso -i "$f"
done
```

Migrations are forward only and numbered. Run them in order.

## Common problems

| Symptom | Cause | Fix |
| --- | --- | --- |
| `MSB1009` on build | Solution file not where you think | Run from the repository root |
| Tests pass locally, fail in CI | Culture dependent formatting | CI runs invariant culture, use `CultureInfo.InvariantCulture` |
| `npm ci` fails | Lock file out of date with `package.json` | `npm install`, commit the lock file |
| Compose sql container restarts | Password does not meet complexity | Eight characters, upper, lower, digit |
| `terraform init` asks for credentials | Not signed in | `az login`, then `az account set --subscription ...` |
