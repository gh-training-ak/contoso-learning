# Runbook: high latency on mentor search

**Alert**: `p95 latency on GET /api/mentors above 800ms for 5 minutes`

## Decide in sixty seconds

1. Open Application Insights, Performance, `GET /api/mentors`.
2. Is the dependency duration high, or the server time?

| What you see | Go to |
| --- | --- |
| SQL dependency over 400ms | [Slow query](#slow-query) |
| Redis dependency over 50ms or failing | [Cache unavailable](#cache-unavailable) |
| Neither dependency slow, server time high | [CPU saturation](#cpu-saturation) |
| Request count 10x normal | [Traffic spike](#traffic-spike) |

## Slow query

Check whether the plan regressed:

```kusto
dependencies
| where timestamp > ago(1h) and type == "SQL"
| summarize p95 = percentile(duration, 95), count() by bin(timestamp, 5m), data
| order by timestamp desc
```

Most common cause is a missing index after a migration. Compare the current indexes
against `database/migrations`. `IX_Mentor_Published_Rate` is the one that matters for
this endpoint.

Mitigation: none safe at 3am other than scaling the database tier one step. Record it
and raise the index as a pull request in the morning.

## Cache unavailable

The service degrades rather than fails when Redis is down, so latency rises but
requests still succeed. Confirm:

```kusto
dependencies
| where timestamp > ago(30m) and type == "Redis"
| summarize failures = countif(success == false), total = count() by bin(timestamp, 1m)
```

If Redis is genuinely down, check the Azure status page for the region before doing
anything. A Premium tier cache failing over takes under a minute.

## CPU saturation

App Service, Metrics, CPU Percentage, split by instance.

If one instance is hot and the others are not, that instance is stuck. Restart it.
If all instances are hot, scale out. Scaling up does not help a request bound workload.

## Traffic spike

Check whether it is one caller:

```kusto
requests
| where timestamp > ago(1h) and name == "GET /api/mentors"
| summarize count() by client_IP
| top 10 by count_
```

The fixed window limiter should already be returning `429` to a single abusive caller.
If one IP is above 100 a minute and not being limited, the limiter is not seeing the
real client address. Check that `X-Forwarded-For` is being honoured.

## After the incident

Open an issue with the incident template. Link the alert, the query you ran and the
action you took. If you scaled anything, say so, because somebody has to scale it back.
