import { useEffect, useState } from "react"
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog"
import { Button } from "@/components/ui/button"
import {
  fetchConferenceRooms,
  updateBooking,
  type BookingResponse,
  type ConferenceRoom,
} from "@/lib/api"

interface EditBookingDialogProps {
  booking: BookingResponse
  onClose: () => void
  onSaved: (updated: BookingResponse) => void
}

export function EditBookingDialog({ booking, onClose, onSaved }: EditBookingDialogProps) {
  const [date, setDate] = useState(booking.dateIso)
  const [timeFrom, setTimeFrom] = useState(booking.timeFrom ?? "09:00")
  const [timeTo, setTimeTo] = useState(booking.timeTo ?? "10:00")
  const [selectedRoomId, setSelectedRoomId] = useState(booking.resourceId)

  const [rooms, setRooms] = useState<ConferenceRoom[]>([])
  const [loadingRooms, setLoadingRooms] = useState(false)

  const [saving, setSaving] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState(false)

  // Reload available rooms whenever date/time changes
  useEffect(() => {
    if (!date || !timeFrom || !timeTo) return
    setLoadingRooms(true)
    setError(null)
    fetchConferenceRooms(booking.locationId, date, timeFrom, timeTo)
      .then((data) => {
        setRooms(data)
        // If previously selected room is no longer in the list, fall back to first available
        const stillAvailable = data.find((r) => r.id === selectedRoomId && !r.occupied)
        if (!stillAvailable) {
          const first = data.find((r) => !r.occupied)
          setSelectedRoomId(first?.id ?? booking.resourceId)
        }
      })
      .catch(() => setError("Räume konnten nicht geladen werden."))
      .finally(() => setLoadingRooms(false))
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [date, timeFrom, timeTo, booking.locationId])

  async function handleSave() {
    setSaving(true)
    setError(null)
    try {
      const updated = await updateBooking(booking.id, {
        type: booking.type,
        resourceId: selectedRoomId,
        locationId: booking.locationId,
        date,
        timeFrom: booking.timeFrom !== null ? timeFrom : undefined,
        timeTo: booking.timeTo !== null ? timeTo : undefined,
      })
      setSuccess(true)
      onSaved(updated)
    } catch (err) {
      setError(err instanceof Error ? err.message : "Unbekannter Fehler beim Speichern.")
    } finally {
      setSaving(false)
    }
  }

  return (
    <Dialog open onOpenChange={(open) => { if (!open) onClose() }}>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Buchung bearbeiten</DialogTitle>
        </DialogHeader>

        {success ? (
          <div className="space-y-4">
            <p className="text-sm text-green-700 rounded-md bg-green-50 border border-green-200 px-4 py-3">
              Buchung wurde erfolgreich geändert.
            </p>
            <DialogFooter>
              <Button onClick={onClose}>Schließen</Button>
            </DialogFooter>
          </div>
        ) : (
          <div className="space-y-4">
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium" htmlFor="edit-date">
                Datum
              </label>
              <input
                id="edit-date"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                className="h-9 rounded-md border bg-background px-3 text-sm"
              />
            </div>

            {booking.timeFrom !== null && (
              <div className="flex gap-4">
                <div className="flex flex-col gap-1.5 flex-1">
                  <label className="text-sm font-medium" htmlFor="edit-timeFrom">
                    Von
                  </label>
                  <input
                    id="edit-timeFrom"
                    type="time"
                    value={timeFrom}
                    onChange={(e) => setTimeFrom(e.target.value)}
                    className="h-9 rounded-md border bg-background px-3 text-sm"
                  />
                </div>
                <div className="flex flex-col gap-1.5 flex-1">
                  <label className="text-sm font-medium" htmlFor="edit-timeTo">
                    Bis
                  </label>
                  <input
                    id="edit-timeTo"
                    type="time"
                    value={timeTo}
                    onChange={(e) => setTimeTo(e.target.value)}
                    className="h-9 rounded-md border bg-background px-3 text-sm"
                  />
                </div>
              </div>
            )}

            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium" htmlFor="edit-room">
                Konferenzraum
              </label>
              {loadingRooms ? (
                <p className="text-sm text-muted-foreground">Räume werden geladen…</p>
              ) : (
                <select
                  id="edit-room"
                  value={selectedRoomId}
                  onChange={(e) => setSelectedRoomId(e.target.value)}
                  className="h-9 rounded-md border bg-background px-3 text-sm"
                >
                  {rooms.map((r) => (
                    <option key={r.id} value={r.id} disabled={r.occupied && r.id !== booking.resourceId}>
                      {r.name}{r.occupied && r.id !== booking.resourceId ? " (belegt)" : ""}
                    </option>
                  ))}
                  {/* Ensure the original room appears even if no longer returned */}
                  {rooms.length === 0 && (
                    <option value={booking.resourceId}>{booking.resource}</option>
                  )}
                </select>
              )}
            </div>

            {error && (
              <p className="rounded-md border border-destructive/50 bg-destructive/10 px-4 py-2 text-sm text-destructive">
                {error}
              </p>
            )}

            <DialogFooter>
              <Button variant="outline" onClick={onClose} disabled={saving}>
                Abbrechen
              </Button>
              <Button onClick={handleSave} disabled={saving || loadingRooms}>
                {saving ? "Wird gespeichert…" : "Speichern"}
              </Button>
            </DialogFooter>
          </div>
        )}
      </DialogContent>
    </Dialog>
  )
}
