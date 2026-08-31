# Contoso Learning

[![CI](https://github.com/gh-training-ak/contoso-learning/actions/workflows/ci.yml/badge.svg)](https://github.com/gh-training-ak/contoso-learning/actions/workflows/ci.yml)
[![CodeQL](https://github.com/gh-training-ak/contoso-learning/actions/workflows/codeql.yml/badge.svg)](https://github.com/gh-training-ak/contoso-learning/actions/workflows/codeql.yml)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

Contoso Learning matches secondary school students with subject mentors. Students search by
subject, price, rating and distance, send a match request, and review the mentor afterwards.

> This repository exists to teach GitHub. The code is real and it builds, but the service is
> not deployed anywhere and the data is invented.

## Contents

- [Architecture](#architecture)
- [Repository layout](#repository-layout)
- [Running it locally](#running-it-locally)
- [Testing](#testing)
- [Branching and releases](#branching-and-releases)
- [Environments](#environments)
- [Documentation](#documentation)

## Architecture

```
                 Angular 18 (web/)
                        |
                        v
          ASP.NET Core 8 API (Contoso.Api)
                        |
         +--------------+--------------+
         |                             |
  Contoso.Infrastructure        rate limiting,
  (repositories, cache,          auth, logging
   audit sink)
         |
  Contoso.Application  (services, interfaces, paging)
         |
  Contoso.Domain       (entities, value objects, rules)
```

Dependencies point inwards only. `Contoso.Domain` has no project references at all. If the
Application layer needs something from the outside world, it declares an interface and
Infrastructure implements it.

| Concern | Choice | Decision record |
| --- | --- | --- |
| Persistence | Azure SQL, Dapper style repositories | [ADR 0002](docs/adr/0002-sql-over-cosmos.md) |
| Caching | Redis, keyed by search criteria | [ADR 0004](docs/adr/0004-cache-search-results.md) |
| Rate limiting | Fixed window, 60 a minute, in process | [ADR 0005](docs/adr/0005-fixed-window-rate-limiting.md) |
| Paging | Envelope with `total` and `hasNext` | [ADR 0003](docs/adr/0003-paged-envelope.md) |
| Hosting | App Service on Linux, Terraform | [ADR 0001](docs/adr/0001-terraform-over-portal.md) |

## Repository layout

```
src/
  Contoso.Domain/          entities, value objects, domain exceptions
  Contoso.Application/     services, repository interfaces, paging
  Contoso.Infrastructure/  in memory repositories, cache, audit sink
  Contoso.Api/             controllers, middleware, composition root
tests/
  Contoso.Tests/           xUnit, one behaviour per test
web/                       Angular 18 standalone components
database/migrations/       forward only SQL, numbered
infra/terraform/           the deployed topology, one stack per environment
infra/bicep/               the same topology expressed in Bicep, for comparison
deploy/docker/             Dockerfile and compose for local work
deploy/k8s/                manifests kept for the container workshop
docs/                      architecture, runbooks, decision records
.github/                   workflows, templates, ownership, Copilot instructions
```

## Running it locally

Prerequisites: .NET SDK 8.0.4xx, Node 20, Docker Desktop.

```bash
git clone https://github.com/gh-training-ak/contoso-learning.git
cd contoso-learning

dotnet restore Contoso.sln
dotnet run --project src/Contoso.Api
```

The API listens on `http://localhost:5214`. There is no Swagger UI, the contract is
[docs/api.md](docs/api.md).

Front end:

```bash
cd web
npm ci
npm start
```

Everything together, including SQL and Redis:

```bash
export SQL_PASSWORD='Choose-A-Strong-One-1'
docker compose -f deploy/docker/docker-compose.yml up --build
```

The default run uses in memory repositories, so you do not need SQL for most work. They
start empty on purpose. Post a mentor before you search for one.

## Testing

```bash
dotnet test Contoso.sln                  # 54 tests
cd web && npm test                       # Angular specs
terraform -chdir=infra/terraform validate
```

Coverage is collected in CI and published as a build artifact. There is no coverage gate,
because a number does not tell you whether the right thing is tested.

## Branching and releases

`main` is always deployable. Work happens on short lived branches and lands through a pull
request. See [CONTRIBUTING](.github/CONTRIBUTING.md) for the prefixes and the merge rules.

Releases are cut from `release/x.y` branches, tagged `vX.Y.Z`, and the tag triggers the
release workflow which builds the container, attests provenance and generates the notes.

## Environments

| Environment | Branch | Approval | URL |
| --- | --- | --- | --- |
| dev | `main` | None, deploys on every green build | https://dev.contoso.example |
| test | `main` | One reviewer | https://test.contoso.example |
| prod | tags only | Two reviewers, 10 minute wait timer | https://learn.contoso.example |

Secrets live in Key Vault. The App Service reads them with its managed identity. Nothing
sensitive is stored as a GitHub secret except the three OIDC values for `azure/login`.

## Documentation

| Document | What it covers |
| --- | --- |
| [Architecture](docs/architecture.md) | How a request flows end to end |
| [Local setup](docs/local-setup.md) | Longer form of the quick start, including SQL |
| [Runbook: high latency](docs/runbooks/high-latency.md) | What to check when p95 goes above 800ms |
| [Runbook: failed deployment](docs/runbooks/failed-deployment.md) | Rolling back a bad release |
| [ADR index](docs/adr/README.md) | Why the code looks like this |
| [Contributing](.github/CONTRIBUTING.md) | Branches, commits, reviews, merge buttons |
| [Security](.github/SECURITY.md) | Supported versions and private reporting |

## Licence

MIT. See [LICENSE](LICENSE).
