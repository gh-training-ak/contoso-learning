---
mode: ask
description: Review the current diff the way this team reviews.
---

Review the staged changes against the house rules.

Check, in order:

1. Does the dependency direction still point inwards (Api to Infrastructure to Application to Domain)?
2. Is every new rule covered by a test that would fail without the change?
3. Are domain rule violations thrown as `DomainException` rather than returned as `null`?
4. Is anything logged that could contain a student name, email or address?
5. Is the database migration additive and safe to run while the old version is live?

Report findings as a list. For each one give the file, the line and the smallest fix.
Say nothing about formatting. The analyser handles that.
