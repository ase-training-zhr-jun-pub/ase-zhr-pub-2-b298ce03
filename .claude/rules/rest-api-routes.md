# REST API Route Design

API routes must use **nouns (resources)**, never verbs.

---

## Resource Naming

- Always **plural nouns**: `bookings`, `locations`, `rooms`, `workplaces`
- lowercase, hyphen-separated for multi-word resources: `conference-rooms`
- Nest sub-resources to express ownership (max 2 levels): `/api/locations/{id}/rooms`
- Flatten deeper nesting: prefer `GET /api/rooms?locationId=1` over `/api/locations/1/floors/2/rooms`

### Do
```
GET    /api/bookings
GET    /api/bookings/{id}
POST   /api/bookings
PUT    /api/bookings/{id}
PATCH  /api/bookings/{id}
DELETE /api/bookings/{id}
GET    /api/locations/{id}/rooms
```

### Don't
```
POST /api/createBooking
GET  /api/getBookings
POST /api/deleteBooking/{id}
POST /api/booking/cancel
GET  /api/booking             ← singular
```

---

## HTTP Methods

| Method   | Semantics                        | Idempotent |
|----------|----------------------------------|------------|
| `GET`    | Read resource(s)                 | yes        |
| `POST`   | Create a new resource            | no         |
| `PUT`    | Replace a resource completely    | yes        |
| `PATCH`  | Partially update a resource      | no         |
| `DELETE` | Remove a resource                | yes        |

State transitions (e.g. cancel, confirm) use a sub-resource noun, not a verb:
```
POST /api/bookings/{id}/cancellation   ← not /api/bookings/{id}/cancel
POST /api/bookings/{id}/confirmation
```

---

## HTTP Status Codes

Always return the semantically correct status code — never return `200 OK` for an error.

| Situation                              | Code                        |
|----------------------------------------|-----------------------------|
| Successful read / update               | `200 OK`                    |
| Resource created                       | `201 Created` + `Location` header |
| No content to return (e.g. DELETE)     | `204 No Content`            |
| Validation / bad input                 | `400 Bad Request`           |
| Missing / invalid auth token           | `401 Unauthorized`          |
| Authenticated but not allowed          | `403 Forbidden`             |
| Resource not found                     | `404 Not Found`             |
| Conflict (e.g. double booking)         | `409 Conflict`              |
| Server-side bug                        | `500 Internal Server Error` |

Error responses must include a JSON body explaining the problem:
```json
{
  "type": "https://example.com/errors/conflict",
  "title": "Booking conflict",
  "status": 409,
  "detail": "The requested room is already booked for this time slot."
}
```
Follow [RFC 9457 Problem Details](https://www.rfc-editor.org/rfc/rfc9457) (`application/problem+json`).

---

## Filtering, Sorting, and Pagination

Use **query parameters** — never encode filters in the path.

```
GET /api/bookings?date=2026-06-17
GET /api/bookings?locationId=1&type=ConferenceRoom
GET /api/bookings?from=2026-06-01&to=2026-06-30

GET /api/rooms?sort=name            ← ascending
GET /api/rooms?sort=-capacity       ← descending (leading minus)

GET /api/bookings?page=2&pageSize=20
GET /api/bookings?cursor=eyJpZCI6NDJ9   ← cursor-based (preferred for large sets)
```

Paginated responses include metadata:
```json
{
  "data": [...],
  "total": 134,
  "page": 2,
  "pageSize": 20
}
```

---

## Request & Response Format

- Always use `Content-Type: application/json`
- Dates: ISO 8601 `yyyy-MM-dd`, times: `HH:mm`
- Property names: **camelCase** in JSON
- `POST` and `PUT` return the created/updated resource in the response body
- `DELETE` returns `204` with no body

---

## Versioning

Version via the URL prefix when breaking changes are necessary:
```
/api/v1/bookings
/api/v2/bookings
```
Avoid versioning individual endpoints; version the whole API surface together.
