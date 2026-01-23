# 0004. Cache search results on criteria

- Status: Accepted
- Date: 2025-06-18
- Deciders: Nina Castellano, Hannah Choi

## Context

p95 on `GET /api/mentors` sat at 1.1 seconds under the Tuesday evening peak. The query
plan was fine. The volume was the problem: the same handful of searches, repeated.
Roughly 70 percent of searches in a one hour sample were one of eleven distinct filter
combinations.

## Options considered

### Output caching middleware

Cheapest to add. Caches the whole response including headers, and we would have to
vary by every query parameter by hand. Invalidation on mentor update is awkward.

### Cache the search result inside the service

The service already builds a deterministic `MentorSearchCriteria`. Giving it a
`CacheKey` makes the cache a one line concern, and we control invalidation.

## Decision

`ICacheStore` in Application, Redis implementation in Infrastructure, keyed by
`MentorSearchCriteria.CacheKey`. Five minute absolute expiry. The key is dropped when
a mentor in the result set is republished.

## Consequences

- p95 fell to 180ms in the same traffic shape.
- A mentor who edits their profile may be stale for up to five minutes in search,
  but never on their own profile page, which is not cached.
- The cache key must change whenever the criteria shape changes. There is a test
  that fails if a new criteria property is not included in the key.
