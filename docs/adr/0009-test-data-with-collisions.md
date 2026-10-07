# 0009. Test data deliberately collides

- Status: Proposed
- Date: 2026-10-07
- Deciders: Grace Underwood, Marco Silva, Sam Whitfield

## Context

Page two of mentor search repeated a row from page one. The bug was in the ordering, which
had no tie break, so two mentors on the same rating came back in a different order on every
call.

Our tests never caught it and could not have. Every mentor in every fixture has a distinct
rating, a non null address and at least one review. The awkward cases are the ones we never
build.

Real data clusters. Once there are enough reviews, half the mentors will sit between 4.4
and 4.6, and a meaningful number will have no reviews and no address at all.

## Options considered

### Property based tests

Generate the awkward cases automatically. Catches more, and a failure reports a generated
case rather than a named scenario, which makes the first reading of a failure slower.

### Hand written fixtures with deliberate collisions

A named set: three mentors on the same rating, one with no reviews, one with a null
address, two on the same price. Slower to extend, obvious when it fails.

## Decision

Hand written fixtures with deliberate collisions, as the default rather than the exception.
A test that wants tidy data asks for it explicitly.

Property based testing is not ruled out. It is a second step once the fixtures stop finding
things.

## Consequences

- Every new test starts from data that is awkward, so ordering and null handling are
  exercised without anyone remembering to.
- Fixtures take longer to read. That is the price, and it is paid once.
- Any test asserting "the first result is X" has to say why X is first, which is a
  reasonable thing to force.
