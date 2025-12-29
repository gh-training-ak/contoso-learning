# 0003. Paged envelope instead of bare arrays

- Status: Accepted
- Date: 2025-04-02
- Deciders: Sam Whitfield, Maya Lindqvist

## Context

`GET /api/mentors` returned a JSON array. The front end had no way to know whether
there were more results, so it rendered "showing 20" with no total and no next page.
One search returned 4,100 rows in a single response during a demo.

## Options considered

### Link header pagination

Standard, used by GitHub. The front end has to parse the header, and our Angular
client would need a custom interceptor. Harder to see in a browser.

### Envelope in the body

`{ items, page, pageSize, total, totalPages, hasNext, hasPrevious }`. Trivial for the
client, visible in Swagger, costs one extra `COUNT` per query.

## Decision

Envelope in the body. `PagedResult<T>` in `Contoso.Application`. Page size is clamped
to 100 server side regardless of what the caller asks for.

## Consequences

- Breaking change for any existing caller. Shipped in 3.0.0 with a migration note.
- One extra count query per search. Measured at under 4ms on the indexed path.
- `total` is computed before paging, so the cache key must include page and page size.
