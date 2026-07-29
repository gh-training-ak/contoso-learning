---
applyTo: "web/**/*.ts"
---

- Standalone components only. No NgModules.
- Inject with `inject()`, not constructor parameters.
- State on components is `signal` or `computed`. No manual change detection calls.
- Subscriptions either use the async pipe or `takeUntilDestroyed`.
- Interfaces are `readonly` where the value is not meant to change.
- No `any`. Use `unknown` and narrow.
