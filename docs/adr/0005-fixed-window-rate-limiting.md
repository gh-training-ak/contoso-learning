# 0005. Fixed window rate limiting in process

- Status: Accepted
- Date: 2025-08-05
- Deciders: Victor Adeyemi, Oliver Grant

## Context

A misconfigured partner integration sent 40 requests a second to the search endpoint
for six hours. Nothing fell over, but the SQL serverless tier auto scaled and the bill
for that day was four times normal.

## Options considered

### Azure Front Door rate limiting

Correct place for it, stops traffic before it costs us compute. Adds a service we do
not otherwise need yet, and the rules are not in this repository.

### Sliding window with Redis

Accurate across instances. Adds a Redis round trip to every request on the hot path.

### Fixed window, in process

One dictionary lookup, no network call. Allows a burst of up to twice the limit at a
window boundary. Each instance counts separately, so the effective limit is the limit
multiplied by the instance count.

## Decision

Fixed window, in process, 100 requests per minute per client. Returns `429` with a
`Retry-After` header.

We accept the boundary burst and the per instance counting. At three instances the
real ceiling is 300 a minute, which is still two orders of magnitude below what hurt us.

## Consequences

- Revisit when we put Front Door in front of the API, and move the limit there.
- The limiter is memory bound. Entries are swept on a timer, not on every request.
- Load tests must account for the per instance behaviour or they will report wrongly.
