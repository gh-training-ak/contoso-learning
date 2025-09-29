# 0006. Hand written mapping, no AutoMapper

- Status: Accepted
- Date: 2025-09-09
- Deciders: Sam Whitfield, Tom Baker

## Context

A pull request proposed AutoMapper to remove roughly 60 lines of mapping code.

## Options considered

### AutoMapper

Less code to write. Mapping errors move from compile time to run time, and the
profiles become a second place to look when a field is wrong. Startup configuration
validation helps but has to be remembered.

### Hand written

More lines. Every mapping is a compile time error if a property is renamed, and
"find all references" works.

## Decision

Hand written. Mapping lives next to the type it maps from, as a static method.

## Consequences

- Renaming a domain property breaks the build, which is what we want.
- About 60 extra lines today, growing slowly. Acceptable.
- If mapping ever exceeds roughly 500 lines, revisit with a source generator rather
  than a reflection based mapper.
