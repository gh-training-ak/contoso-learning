# Release 3.2.0 checklist

Release manager: Grace Underwood

| Check | Status | Notes |
| --- | --- | --- |
| CI green on the branch | Done | |
| Migrations applied to test | Done | 0004 applied 22 September |
| Search regression pass | Done | 40 scripted searches, no ordering drift |
| Availability smoke test | Done | Overlap refused, clash named |
| Rate limit behaviour | Done | 429 at 61 requests, Retry-After correct |
| Rollback rehearsed | Done | Previous digest redeployed in 4 minutes |
| Release notes drafted | Done | Generated, then trimmed by hand |

## Known issues shipping with this release

- The rate limiter keys on the remote address rather than `X-Forwarded-For`.
  Tracked, not a regression, present since 3.1.0.
