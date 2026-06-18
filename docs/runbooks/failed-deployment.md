# Runbook: failed deployment

**Symptom**: the deploy workflow failed, or it succeeded and the app is unhealthy.

## If the workflow failed

Nothing was deployed if `terraform apply` failed before any resource changed. Read the
plan output in the job log. The common failures:

| Error | Meaning | Action |
| --- | --- | --- |
| `Error acquiring the state lock` | A previous run died holding the lock | Confirm no apply is running, then `terraform force-unlock <id>` |
| `AuthorizationFailed` | The OIDC federated credential is missing a role | Platform team grants it, do not use a secret as a workaround |
| `ResourceNotFound` on a resource we own | Somebody deleted it in the portal | Re run the apply, it will recreate it |
| `409 Conflict` on the app service | A deployment is already in progress | Wait, then re run |

## If it succeeded but the app is unhealthy

1. `GET /healthz` on the environment URL. If it answers, the app started.
2. App Service, Deployment Center, check which build is actually running.
3. Log stream for startup exceptions. A missing Key Vault reference shows as a
   configuration binding failure in the first seconds.

## Rolling back

App Service keeps the previous deployment. Fastest path:

```bash
az webapp deployment list-publishing-profiles --name app-contoso-prod-api-xxxx -g rg-contoso-prod
az webapp deployment source show --name app-contoso-prod-api-xxxx -g rg-contoso-prod
```

Re run the release workflow with the previous tag. That republishes the known good
container digest. Do not revert the merge commit on `main` to roll back, that produces
a second deploy and leaves the history confusing.

If the bad release included a database migration, rolling the app back is not enough.
Migrations are additive, so the previous version still works against the new schema.
That is the reason for the rule. If somebody shipped a destructive migration, stop and
escalate, restoring from backup is not a solo decision.

## After

Incident issue, timeline, and a check on whether CI could have caught it. Most failed
deployments we have had were caught by nothing because nothing ran the app before prod.
