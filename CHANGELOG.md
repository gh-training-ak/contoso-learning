# Changelog

Notable changes per release. Generated notes live on the GitHub releases page, this
file carries the summary and the migration notes.

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
