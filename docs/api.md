# API reference

Base URL in development is `http://localhost:5214`.

All endpoints return `application/json`. Failures return `application/problem+json`.
There is no Swagger UI. The contract is this file and the `ProducesResponseType`
attributes on the controllers, which is the pair that has to stay in step.

## GET /api/mentors

Search active mentors.

| Parameter | Type | Notes |
| --- | --- | --- |
| `subject` | enum | One of the nine values from `GET /api/mentors/subjects` |
| `meetingType` | enum | `Online`, `InPerson`, `Either` |
| `maxHourlyRate` | decimal | Inclusive |
| `minimumRating` | decimal | Inclusive, 0 to 5 |
| `page` | int | 1 based. Anything below 1 is treated as 1. |
| `pageSize` | int | Defaults to 20, clamped server side to 100 |

```json
{
  "items": [
    {
      "id": "0b3f6b1e-9c0a-4f1e-93a0-7b2c0e4f1a22",
      "displayName": "A. Mentor",
      "hourlyRate": 32.50,
      "currency": "GBP",
      "rating": 4.33,
      "reviewCount": 23,
      "distanceKm": null
    }
  ],
  "page": 1,
  "pageSize": 20,
  "total": 143,
  "totalPages": 8,
  "hasNext": true,
  "hasPrevious": false
}
```

| Status | When |
| --- | --- |
| 200 | Always, including zero results |
| 429 | Over 60 requests in the current minute. `Retry-After` holds the seconds left. |

`distanceKm` is only populated when the caller supplied an origin. The HTTP surface does
not take one yet, so it is `null` from this endpoint. The service layer supports it,
see `MentorSearchCriteria.Near`.

Results are ordered by rating descending, then by distance ascending.

## GET /api/mentors/{id}

| Status | When |
| --- | --- |
| 200 | Found and active |
| 404 | Unknown id, or the mentor is soft deleted |

## GET /api/mentors/subjects

Returns the subject enum names as an array of strings. The front end builds the filter
dropdown from this rather than hard coding a second copy of the list.

## POST /api/match-requests

```json
{ "studentId": "...", "mentorId": "..." }
```

| Status | When |
| --- | --- |
| 201 | Created. `Location` points back at the create action. |
| 404 | Unknown mentor |
| 409 | A request between this student and mentor is already pending |

## PATCH /api/match-requests/{id}

One endpoint answers the request, rather than two verbs that can disagree.

```json
{ "accept": false, "reason": "Full until September" }
```

| Status | When |
| --- | --- |
| 200 | Answered, returns the updated request |
| 404 | Unknown request |
| 409 | Already answered. The state machine only moves out of `Pending` once. |

A decline without a reason is rejected by the domain, because the student sees it.

## GET /healthz

Liveness only. No dependency is checked, so a `200` here means the process is up, not
that the database is reachable.
