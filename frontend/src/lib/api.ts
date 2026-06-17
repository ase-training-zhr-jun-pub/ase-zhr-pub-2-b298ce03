declare const __BACKEND_URL__: string

const BASE = __BACKEND_URL__

export type Location = {
  id: string
  name: string
}

export type Workplace = {
  id: string
  name: string
  floor: string
  equipment: string[]
  occupied: boolean
  locationId: string
}

export type ConferenceRoom = {
  id: string
  name: string
  capacity: number
  equipment: string[]
  occupied: boolean
  locationId: string
}

export type BookingResponse = {
  id: string
  type: string
  resource: string
  location: string
  date: string
  timeRange: string
}

export type BookingRequest = {
  type: string
  resourceId: string
  locationId: string
  userId: string
  date: string
  timeFrom?: string
  timeTo?: string
}

async function get<T>(path: string): Promise<T> {
  const res = await fetch(`${BASE}${path}`)
  if (!res.ok) throw new Error(`${res.status} ${res.statusText}`)
  return res.json() as Promise<T>
}

function debug(val: any) {
  console.log(val)
}

export function fetchLocations(): Promise<Location[]> {
  return get("/api/locations")
}

export function fetchWorkplaces(locationId?: string, date?: string): Promise<Workplace[]> {
  const params = new URLSearchParams()
  if (locationId) params.set("locationId", locationId)
  if (date) params.set("date", date)
  const qs = params.toString()
  return get(`/api/workplaces${qs ? `?${qs}` : ""}`)
}

export function fetchConferenceRooms(
  locationId?: string,
  date?: string,
  timeFrom?: string,
  timeTo?: string,
): Promise<ConferenceRoom[]> {
  const params = new URLSearchParams()
  if (locationId) params.set("locationId", locationId)
  if (date) params.set("date", date)
  if (timeFrom) params.set("timeFrom", timeFrom)
  if (timeTo) params.set("timeTo", timeTo)
  const qs = params.toString()
  return get(`/api/conference-rooms${qs ? `?${qs}` : ""}`)
}

export function fetchBookings(userId?: string): Promise<BookingResponse[]> {
  const params = new URLSearchParams()
  if (userId) params.set("userId", userId)
  const qs = params.toString()
  return get(`/api/bookings${qs ? `?${qs}` : ""}`)
}

export async function createBooking(req: BookingRequest): Promise<BookingResponse> {
  const res = await fetch(`${BASE}/api/bookings`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(req),
  })
  if (!res.ok) {
    const err = await res.json().catch(() => ({})) as { error?: string }
    throw new Error(err.error ?? `${res.status} ${res.statusText}`)
  }
  return res.json() as Promise<BookingResponse>
}

export async function deleteBooking(id: string): Promise<void> {
  const r = await fetch(`${BASE}/api/bookings/${id}`, { method: "DELETE" })
  if (!r.ok) throw new Error(`${r.status} ${r.statusText}`)
}

export type { Location, Workplace, ConferenceRoom, BookingResponse, BookingRequest }
