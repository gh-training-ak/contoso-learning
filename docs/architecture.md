# Architecture

## Request path

A search request walks this path:

1. `MentorsController.Search` binds the query string into `MentorSearchCriteria`.
   Page size is clamped to 100 here, not in the service, because it is a transport concern.
2. `FixedWindowLimiter` has already run as middleware. If the caller is over the limit
   the controller is never reached and the response is `429` with `Retry-After`.
3. `MentorSearchService.SearchAsync` asks `ICacheStore` for `criteria.CacheKey`.
4. On a miss it calls `IMentorRepository.SearchAsync`, which applies subject, price,
   rating and meeting type filters, then distance if an origin was supplied.
5. Results are ordered by rating descending, then by review count, so a mentor with
   one five star review does not outrank one with forty.
6. The page is wrapped in `PagedResult<T>` and written to the cache for five minutes.
7. `IAuditSink` records the search. The sink is fire and forget, a failure there never
   fails the request.

## Why the layers are this way

`Contoso.Domain` has no project references. That is enforced by the build, not by
convention: adding a reference to Application from Domain creates a cycle and fails.

Everything the Application layer needs from the outside is an interface it owns:

| Interface | Lives in | Implemented by |
| --- | --- | --- |
| `IMentorRepository` | Application | `InMemoryMentorRepository`, later `SqlMentorRepository` |
| `IMatchRequestRepository` | Application | `InMemoryMatchRequestRepository` |
| `ICacheStore` | Application | `InMemoryCacheStore`, Redis in deployed environments |
| `IAuditSink` | Application | `InMemoryAuditSink`, Event Hub in prod |

The practical payoff: the entire test suite runs with no database, no Redis and no
network, in under a second.

## Domain rules worth knowing

- `Money` refuses to add two different currencies. This is a `record struct`, so it
  costs nothing at runtime and catches the mistake at the point it happens.
- `MatchRequest` can only move out of `Pending` once. A second accept throws.
- `Review` requires a completed match. The factory enforces it, so there is no path
  that creates an invalid review.
- `ScheduleSlot` rejects a slot that ends before it starts, including slots that would
  cross midnight. Mentors who teach late split the slot.

## What is deliberately missing

- No authentication. The API trusts a header for the caller identity. Real auth is the
  subject of a different workshop.
- No EF Core. Repositories are in memory. The SQL schema exists and is the shape the
  real repositories would target.
- No background jobs. The audit sink writes synchronously to memory.
