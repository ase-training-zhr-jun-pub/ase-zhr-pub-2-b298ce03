# Calvin Booking Service – Load Tests

Performance/load tests for the Calvin booking service REST API, built with [NBomber](https://nbomber.io/).

## Prerequisites

- .NET 10 SDK
- Calvin backend running (see below)

## Start the backend

```bash
cd backend
dotnet run
```

The backend starts on `http://localhost:5000` by default.

## Run the load tests

```bash
dotnet run --project load-tests/
```

To target a different backend URL, set the `CALVIN_BASE_URL` environment variable:

```bash
CALVIN_BASE_URL=http://my-server:5000 dotnet run --project load-tests/
```

## Scenarios

| Scenario             | Endpoint                            | VUs | Duration |
|----------------------|-------------------------------------|-----|----------|
| `list-bookings`      | `GET /api/bookings`                 | 10  | 30 s     |
| `list-rooms`         | `GET /api/conference-rooms`         | 10  | 30 s     |
| `create-booking`     | `POST /api/bookings`                |  5  | 30 s     |
| `conflict-detection` | `POST /api/bookings` (same slot)    |  5  | 30 s     |

**`create-booking`** uses unique future dates (2027+) per virtual user and iteration
to avoid conflicts — all requests are expected to return `201 Created`.

**`conflict-detection`** intentionally sends all virtual users to the same
resource/date/time. The first writer receives `201 Created`; subsequent ones
receive `409 Conflict`. Both are treated as successful test outcomes. This
verifies that the backend handles concurrent booking attempts safely and never
returns a `5xx` error.

## Reports

After the run, NBomber writes an HTML report to `load-tests/load-test-results/`.
