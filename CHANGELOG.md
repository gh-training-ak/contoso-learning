# Changelog

Notable changes per release. Generated notes live on the GitHub releases page, this
file carries the summary and the migration notes.

## [3.2.0] - 2026-09-24

### Added
- Mentor availability calendar with overlap detection (#412)
- `GET /api/mentors/subjects` so the front end stops hard coding the list (#431)

### Changed
- Search ordering now breaks ties by review count (#428)

### Fixed
- Distance filter no longer drops mentors sitting exactly on the radius (#434)

## [3.1.0] - 2026-06-11

### Added
- Fixed window rate limiting, 100 requests per minute, `429` with `Retry-After` (#355)
- Audit sink records search and match request activity (#361)

### Security
- Push protection enabled on the repository, secret scanning alerts triaged

## [3.0.0] - 2026-03-02

### Breaking
- `GET /api/mentors` returns a paged envelope rather than a bare array.
  Callers must read `items`. See [ADR 0003](docs/adr/0003-paged-envelope.md).

### Added
- Redis cache on search results, p95 from 1.1s to 180ms (#298)

### Removed
- `GET /api/mentors/all`, which had no paging and no limit

## [2.1.0] - 2025-11-18

### Added
- Review submission with a completed match requirement (#211)
- Mentor rating view in SQL (#214)

## [2.0.0] - 2025-08-27

### Breaking
- Money is now an object with `amount` and `currency` rather than a bare decimal

### Added
- Match request accept and decline with a mandatory decline reason (#152)

## [1.0.0] - 2025-04-15

First release. Mentor search, student registration, Terraform for all three environments.
