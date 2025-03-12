# 0002. Azure SQL rather than Cosmos DB

- Status: Accepted
- Date: 2025-02-11
- Deciders: Ruth Ellory, Sam Whitfield, Alex Morgan

## Context

The core query is "mentors teaching subject X, under price Y, rated above Z, within
N kilometres, sorted by rating". That is four predicates and a sort, over a set that
will stay in the low hundreds of thousands for years. Reviews aggregate per mentor.

## Options considered

### Cosmos DB

Attractive for the mentor document shape and for geo distribution we do not need yet.
The aggregate rating query becomes either a stored aggregate we maintain by hand or a
cross partition query that costs real RU. Multi predicate filtering needs composite
indexes we would be guessing at.

### Azure SQL

The filter and sort is a single indexed query. Aggregates come from a view. The team
can read a query plan. Serverless tier keeps dev and test cheap.

## Decision

Azure SQL. General Purpose serverless in dev and test, provisioned in prod.

## Consequences

- Schema changes need migrations. We accept that cost for the query flexibility.
- Distance filtering happens in the application for now, because the geography type
  adds complexity we do not need at this size. Revisit above 500k mentors.
- If we ever need multi region writes this decision gets revisited, not patched.
