---
applyTo: "infra/**/*.tf"
---

- Every variable has a `description` and a `type`. Add `validation` when the set is closed.
- Environment differences live in `locals.sizing`, never in `count` on a resource.
- Tags come from `local.base_tags`. Do not hand write tag maps on resources.
- Never commit `.tfvars` containing secrets. Secrets come from Key Vault at run time.
- Run `terraform fmt -recursive` before you push. CI fails on unformatted files.
