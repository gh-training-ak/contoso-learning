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

