# Contoso Learning

Matching service for students and mentors. ASP.NET Core 8 API, Angular 18 front end,
Azure SQL, Redis, deployed to Azure App Service through Terraform.

## Architecture

Four projects, dependencies point inwards only:

- `Contoso.Domain` has no project references. Entities, value objects, domain rules.
- `Contoso.Application` references Domain. Service orchestration and repository interfaces.
- `Contoso.Infrastructure` references Application. Repository implementations, cache, audit.
- `Contoso.Api` references Infrastructure. Controllers, middleware, composition root.

Never reference Infrastructure from Application. If you need something from the outside,
add an interface to Application and implement it in Infrastructure.

## Conventions

- Target framework is `net8.0`. `Nullable` and `TreatWarningsAsErrors` are on.
- Money is the `Money` record struct, never `decimal` on its own. Mixing currencies throws.
- Domain rules throw `DomainException`. Controllers translate that into `400`.
- Entities have private setters and expose behaviour, not property assignment.
- Collections on entities are exposed as `IReadOnlyCollection<T>`.
- Tests are xUnit with `Assert`, one behaviour per test, named `Method_Condition_Outcome`.

## Things we do not want

- No comments that restate the code.
- No `async void`. No `.Result` or `.Wait()`.
- No new NuGet packages without an ADR in `docs/adr`.
- No raw SQL string concatenation. Parameters only.

## Useful commands

```bash
dotnet build Contoso.sln -c Release
dotnet test Contoso.sln --no-build
cd web && npm ci && npm test
terraform -chdir=infra/terraform validate
```
