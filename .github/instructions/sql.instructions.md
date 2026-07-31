---
applyTo: "database/**/*.sql"
---

- Migrations are forward only and numbered. Never edit a migration that has shipped.
- Every constraint is named. `PK_`, `FK_`, `UQ_`, `CK_`, `DF_`, `IX_`, `UX_`.
- Adding a non nullable column needs a default or a backfill step in the same file.
- Dropping a column is a two release change. Stop writing first, drop next release.
