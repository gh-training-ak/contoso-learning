---
applyTo: "**/*.cs"
---

- File scoped namespaces. One public type per file.
- `var` only when the type is on the right hand side of the assignment.
- Guard clauses first, happy path last, no `else` after a `throw` or `return`.
- Prefer `IReadOnlyList<T>` over `List<T>` on public surfaces.
- `CancellationToken` is the last parameter on every async method and is passed down.
- Suffix async methods with `Async`.
- Throw `DomainException` for rule violations, `ArgumentException` for programmer error.
