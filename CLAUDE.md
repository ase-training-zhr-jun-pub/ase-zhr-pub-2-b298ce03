# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

**Calvin** is INNOQ's internal room and workplace booking system for 8 office locations. The system has two components:

- **`frontend/`** — React 19 SPA (Vite, TypeScript, Tailwind CSS v4, ShadCN UI)
- **`backend/`** — ASP.NET Minimal API (.NET 10, C#) called `Calvin.BookingService`

## Commands

### Frontend (`frontend/`)

```bash
cd frontend && npm run dev       # Start dev server (port 5173)
cd frontend && npm run build     # Type-check + production build
cd frontend && npm run lint      # ESLint
```

Adding ShadCN components:
```bash
cd frontend && npx shadcn@latest add <name> --overwrite
cd frontend && npx shadcn@latest list   # see available components
```

### Backend (`backend/`)

```bash
cd backend && dotnet run         # Start API server (port 5000)
cd backend && dotnet build       # Build
```

The backend has no automated tests currently. Interactive API docs are served at `/scalar/v1` when running.

## Architecture

### Frontend

The SPA uses `react-router-dom` with all routes defined in `frontend/src/App.tsx`. Pages live in `frontend/src/pages/`, shared UI components in `frontend/src/components/`, and ShadCN-generated components in `frontend/src/components/ui/`.

**Important:** The frontend currently operates without a live backend. All resource master data (locations, rooms, workplaces) is mocked in `frontend/src/lib/mock-data.ts`. Only bookings eventually go to the backend REST API; during prototyping, those are also mocked.

When the frontend does make API calls to the backend, use **relative paths without a leading slash** (e.g. `fetch("api/bookings")` not `fetch("/api/bookings")`). This is required for the Crucible proxy environment.

**Styling:** Tailwind v4 uses CSS-only config via `@theme` — there is no `tailwind.config.js`. Use `cn()` from `@/lib/utils` to merge Tailwind classes. The `@/` alias maps to `frontend/src/`.

### Backend

`Calvin.BookingService` is a .NET 10 Minimal API. The architecture follows a simple layered structure:

- **`Domain/`** — C# records (plain data: `Booking`, `ConferenceRoom`, `Location`, `User`, `Workplace`)
- **`Data/`** — `InMemoryStore` (singleton, thread-safe via locks) + `SeedData` with realistic INNOQ office data
- **`Endpoints/`** — one static class per resource (`MapBookings`, `MapLocations`, etc.), registered as extension methods on `IEndpointRouteBuilder`
- **`Http/`** — request/response DTOs separate from domain records

All state is in-memory and resets on restart. `InMemoryStore.TryAddBooking` performs conflict detection (overlapping time ranges on the same resource/date) inside a lock.

CORS: In the Crucible proxy environment, frontend and backend share the same origin, so no CORS preflight occurs. For local dev, `appsettings.json` allows `http://localhost:5173`. If `VSCODE_PROXY_URI` is set, that origin is added automatically.

### Frontend ↔ Backend contract

The SPA sends/receives JSON. Dates use ISO 8601 (`yyyy-MM-dd`), times use `HH:mm`. A `null` `timeFrom`/`timeTo` means all-day ("Ganztägig"). The `type` field on a booking is either `"ConferenceRoom"` or `"Workplace"`.

## Key Conventions

- **Language:** All code (names, routes, JSON keys, file names) in English. German only in UI display strings, code comments on domain concepts, and `docs/`.
- **Commits:** Follow Conventional Commits.
- **Ubiquitous language:** Domain terms come from `docs/produkt/glossar.md` — use that wording in UI labels and documentation.
- **Documentation:** Check `docs/` before making architectural decisions. ADRs live in `docs/arc42/adrs/`.
