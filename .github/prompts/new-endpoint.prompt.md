---
mode: agent
description: Add a controller endpoint with the full vertical slice.
---

Add a new endpoint. Ask for the route and the behaviour, then produce, in this order:

1. The domain change, if the rule belongs in `Contoso.Domain`.
2. The repository method on the interface in `Contoso.Application`.
3. The in memory implementation in `Contoso.Infrastructure`.
4. The service method that orchestrates it.
5. The controller action with the right status codes and `ProducesResponseType`.
6. Unit tests for the domain rule and the service.

Keep each step small enough to review on its own. Stop after each file and wait.
