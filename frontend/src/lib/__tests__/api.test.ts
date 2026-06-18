import { describe, it, expect, vi, beforeEach, afterEach } from "vitest"
import {
  fetchLocations,
  fetchWorkplaces,
  fetchConferenceRooms,
  fetchBookings,
  createBooking,
  deleteBooking,
  cancelBooking,
  updateBooking,
  type Location,
  type Workplace,
  type ConferenceRoom,
  type BookingResponse,
  type BookingRequest,
  type BookingUpdateRequest,
} from "../api"

// ---------------------------------------------------------------------------
// Helpers
// ---------------------------------------------------------------------------

/** Minimal BookingResponse fixture */
const bookingFixture: BookingResponse = {
  id: "b1",
  type: "ConferenceRoom",
  resource: "Rheinblick",
  location: "Köln",
  date: "18.06.2026",
  timeRange: "10:00 – 11:00",
  resourceId: "KR-KOE-01",
  locationId: "koeln",
  userId: "alice",
  dateIso: "2026-06-18",
  timeFrom: "10:00",
  timeTo: "11:00",
  title: "Team-Meeting",
  notes: null,
  status: "Active",
  isPast: false,
  equipment: ["Bildschirm"],
}

function mockFetchOk(body: unknown, status = 200) {
  return vi.fn().mockResolvedValue({
    ok: true,
    status,
    statusText: "OK",
    json: () => Promise.resolve(body),
  })
}

function mockFetchError(status: number, statusText: string, body?: unknown) {
  return vi.fn().mockResolvedValue({
    ok: false,
    status,
    statusText,
    json: () => (body ? Promise.resolve(body) : Promise.reject(new Error("no body"))),
  })
}

// ---------------------------------------------------------------------------
// Setup / Teardown
// ---------------------------------------------------------------------------

beforeEach(() => {
  vi.stubGlobal("fetch", undefined)
})

afterEach(() => {
  vi.restoreAllMocks()
})

// ---------------------------------------------------------------------------
// fetchLocations
// ---------------------------------------------------------------------------

describe("fetchLocations()", () => {
  it("returns an array of locations on success", async () => {
    const locations: Location[] = [
      { id: "koeln", name: "Köln" },
      { id: "berlin", name: "Berlin" },
    ]
    vi.stubGlobal("fetch", mockFetchOk(locations))

    const result = await fetchLocations()

    expect(result).toEqual(locations)
    expect(fetch).toHaveBeenCalledOnce()
    expect(fetch).toHaveBeenCalledWith(expect.stringContaining("/api/locations"))
  })

  it("throws when the server responds with an error", async () => {
    vi.stubGlobal("fetch", mockFetchError(500, "Internal Server Error"))

    await expect(fetchLocations()).rejects.toThrow("500 Internal Server Error")
  })
})

// ---------------------------------------------------------------------------
// fetchWorkplaces
// ---------------------------------------------------------------------------

describe("fetchWorkplaces()", () => {
  const workplaces: Workplace[] = [
    {
      id: "wp-1",
      name: "Desk A1",
      floor: "2. OG",
      equipment: ["Monitor"],
      occupied: false,
      locationId: "koeln",
    },
  ]

  it("fetches all workplaces without filters", async () => {
    vi.stubGlobal("fetch", mockFetchOk(workplaces))

    const result = await fetchWorkplaces()

    expect(result).toEqual(workplaces)
    const url = (fetch as ReturnType<typeof vi.fn>).mock.calls[0][0] as string
    expect(url).toContain("/api/workplaces")
    expect(url).not.toContain("?")
  })

  it("passes locationId and date as query parameters", async () => {
    vi.stubGlobal("fetch", mockFetchOk(workplaces))

    await fetchWorkplaces("koeln", "2026-06-18")

    const url = (fetch as ReturnType<typeof vi.fn>).mock.calls[0][0] as string
    expect(url).toContain("locationId=koeln")
    expect(url).toContain("date=2026-06-18")
  })

  it("throws on HTTP error", async () => {
    vi.stubGlobal("fetch", mockFetchError(404, "Not Found"))

    await expect(fetchWorkplaces("unknown")).rejects.toThrow("404 Not Found")
  })
})

// ---------------------------------------------------------------------------
// fetchConferenceRooms
// ---------------------------------------------------------------------------

describe("fetchConferenceRooms()", () => {
  const rooms: ConferenceRoom[] = [
    {
      id: "KR-KOE-01",
      name: "Rheinblick",
      capacity: 8,
      equipment: ["Bildschirm"],
      occupied: false,
      locationId: "koeln",
    },
  ]

  it("fetches conference rooms without filters", async () => {
    vi.stubGlobal("fetch", mockFetchOk(rooms))

    const result = await fetchConferenceRooms()

    expect(result).toEqual(rooms)
    const url = (fetch as ReturnType<typeof vi.fn>).mock.calls[0][0] as string
    expect(url).toContain("/api/conference-rooms")
    expect(url).not.toContain("?")
  })

  it("passes all optional filters as query parameters", async () => {
    vi.stubGlobal("fetch", mockFetchOk(rooms))

    await fetchConferenceRooms("koeln", "2026-06-18", "10:00", "11:00")

    const url = (fetch as ReturnType<typeof vi.fn>).mock.calls[0][0] as string
    expect(url).toContain("locationId=koeln")
    expect(url).toContain("date=2026-06-18")
    expect(url).toContain("timeFrom=10%3A00")
    expect(url).toContain("timeTo=11%3A00")
  })

  it("throws on HTTP error", async () => {
    vi.stubGlobal("fetch", mockFetchError(503, "Service Unavailable"))

    await expect(fetchConferenceRooms()).rejects.toThrow("503 Service Unavailable")
  })
})

// ---------------------------------------------------------------------------
// fetchBookings
// ---------------------------------------------------------------------------

describe("fetchBookings()", () => {
  it("fetches all bookings without userId filter", async () => {
    vi.stubGlobal("fetch", mockFetchOk([bookingFixture]))

    const result = await fetchBookings()

    expect(result).toEqual([bookingFixture])
    const url = (fetch as ReturnType<typeof vi.fn>).mock.calls[0][0] as string
    expect(url).toContain("/api/bookings")
    expect(url).not.toContain("userId")
  })

  it("passes userId as query parameter", async () => {
    vi.stubGlobal("fetch", mockFetchOk([bookingFixture]))

    await fetchBookings("alice")

    const url = (fetch as ReturnType<typeof vi.fn>).mock.calls[0][0] as string
    expect(url).toContain("userId=alice")
  })

  it("throws on HTTP error", async () => {
    vi.stubGlobal("fetch", mockFetchError(401, "Unauthorized"))

    await expect(fetchBookings()).rejects.toThrow("401 Unauthorized")
  })
})

// ---------------------------------------------------------------------------
// createBooking
// ---------------------------------------------------------------------------

describe("createBooking()", () => {
  const request: BookingRequest = {
    type: "ConferenceRoom",
    resourceId: "KR-KOE-01",
    locationId: "koeln",
    userId: "alice",
    date: "2026-06-18",
    timeFrom: "10:00",
    timeTo: "11:00",
  }

  it("sends a POST request and returns the created booking", async () => {
    vi.stubGlobal("fetch", mockFetchOk(bookingFixture, 201))

    const result = await createBooking(request)

    expect(result).toEqual(bookingFixture)
    const [url, init] = (fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [string, RequestInit]
    expect(url).toContain("/api/bookings")
    expect(init.method).toBe("POST")
    expect(init.headers).toMatchObject({ "Content-Type": "application/json" })
    expect(JSON.parse(init.body as string)).toEqual(request)
  })

  it("throws with the server error message on conflict", async () => {
    vi.stubGlobal("fetch", mockFetchError(409, "Conflict", { error: "Room already booked" }))

    await expect(createBooking(request)).rejects.toThrow("Room already booked")
  })

  it("falls back to status text when no error field in body", async () => {
    vi.stubGlobal("fetch", mockFetchError(500, "Internal Server Error", {}))

    await expect(createBooking(request)).rejects.toThrow("500 Internal Server Error")
  })
})

// ---------------------------------------------------------------------------
// deleteBooking
// ---------------------------------------------------------------------------

describe("deleteBooking()", () => {
  it("sends a DELETE request and resolves without a value", async () => {
    vi.stubGlobal("fetch", vi.fn().mockResolvedValue({ ok: true, status: 204, statusText: "No Content" }))

    await expect(deleteBooking("b1")).resolves.toBeUndefined()

    const [url, init] = (fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [string, RequestInit]
    expect(url).toContain("/api/bookings/b1")
    expect(init.method).toBe("DELETE")
  })

  it("throws on HTTP error", async () => {
    vi.stubGlobal("fetch", mockFetchError(404, "Not Found"))

    await expect(deleteBooking("missing")).rejects.toThrow("404 Not Found")
  })
})

// ---------------------------------------------------------------------------
// cancelBooking
// ---------------------------------------------------------------------------

describe("cancelBooking()", () => {
  it("sends POST to the cancellation sub-resource and returns updated booking", async () => {
    const cancelled = { ...bookingFixture, status: "Cancelled" }
    vi.stubGlobal("fetch", mockFetchOk(cancelled))

    const result = await cancelBooking("b1")

    expect(result.status).toBe("Cancelled")
    const [url, init] = (fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [string, RequestInit]
    expect(url).toContain("/api/bookings/b1/cancellation")
    expect(init.method).toBe("POST")
  })

  it("throws with detail message from server on error", async () => {
    vi.stubGlobal("fetch", mockFetchError(422, "Unprocessable Entity", { detail: "Booking is already cancelled" }))

    await expect(cancelBooking("b1")).rejects.toThrow("Booking is already cancelled")
  })

  it("falls back to status text when no detail field in body", async () => {
    vi.stubGlobal("fetch", mockFetchError(500, "Internal Server Error", {}))

    await expect(cancelBooking("b1")).rejects.toThrow("500 Internal Server Error")
  })
})

// ---------------------------------------------------------------------------
// updateBooking
// ---------------------------------------------------------------------------

describe("updateBooking()", () => {
  const updateRequest: BookingUpdateRequest = {
    type: "ConferenceRoom",
    resourceId: "KR-KOE-01",
    locationId: "koeln",
    date: "2026-06-19",
    timeFrom: "14:00",
    timeTo: "15:00",
    title: "Updated Meeting",
  }

  it("sends a PUT request and returns the updated booking", async () => {
    const updated = { ...bookingFixture, dateIso: "2026-06-19", timeFrom: "14:00", timeTo: "15:00" }
    vi.stubGlobal("fetch", mockFetchOk(updated))

    const result = await updateBooking("b1", updateRequest)

    expect(result.dateIso).toBe("2026-06-19")
    const [url, init] = (fetch as ReturnType<typeof vi.fn>).mock.calls[0] as [string, RequestInit]
    expect(url).toContain("/api/bookings/b1")
    expect(init.method).toBe("PUT")
    expect(init.headers).toMatchObject({ "Content-Type": "application/json" })
    expect(JSON.parse(init.body as string)).toEqual(updateRequest)
  })

  it("throws with detail message from server on conflict", async () => {
    vi.stubGlobal("fetch", mockFetchError(409, "Conflict", { detail: "Time slot unavailable" }))

    await expect(updateBooking("b1", updateRequest)).rejects.toThrow("Time slot unavailable")
  })

  it("falls back to status text when no detail field in body", async () => {
    vi.stubGlobal("fetch", mockFetchError(400, "Bad Request", {}))

    await expect(updateBooking("b1", updateRequest)).rejects.toThrow("400 Bad Request")
  })
})
