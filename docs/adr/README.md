# Architecture decision records

One file per decision. Numbered, never renumbered, never deleted. A decision that is
reversed gets a new record that supersedes the old one, and the old one is marked.

| Number | Decision | Status |
| --- | --- | --- |
| [0001](0001-terraform-over-portal.md) | Terraform owns the infrastructure | Accepted |
| [0002](0002-sql-over-cosmos.md) | Azure SQL rather than Cosmos DB | Accepted |
| [0003](0003-paged-envelope.md) | Paged envelope instead of bare arrays | Accepted |
| [0004](0004-cache-search-results.md) | Cache search results on criteria | Accepted |
| [0005](0005-fixed-window-rate-limiting.md) | Fixed window rate limiting in process | Accepted |
| [0006](0006-no-automapper.md) | Hand written mapping, no AutoMapper | Accepted |
| [0007](0007-trunk-based-with-release-branches.md) | Trunk based with release branches | Accepted |

Use [the template](0000-template.md). Keep it to one page.
