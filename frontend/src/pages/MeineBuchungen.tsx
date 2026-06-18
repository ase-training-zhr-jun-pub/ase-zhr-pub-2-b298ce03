import { useEffect, useState } from "react"
import { Armchair, DoorOpen, Pencil } from "lucide-react"
import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { fetchBookings, type BookingResponse } from "@/lib/api"
import { CURRENT_USER_ID } from "@/lib/current-user"
import { EditBookingDialog } from "@/components/EditBookingDialog"

export function MeineBuchungen() {
  const [bookings, setBookings] = useState<BookingResponse[]>([])
  const [loading, setLoading] = useState(true)
  const [editingBooking, setEditingBooking] = useState<BookingResponse | null>(null)

  useEffect(() => {
    fetchBookings(CURRENT_USER_ID)
      .then(setBookings)
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

  function handleSaved(updated: BookingResponse) {
    setBookings((prev) => prev.map((b) => (b.id === updated.id ? updated : b)))
    setEditingBooking(null)
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Meine Buchungen</h1>
        <p className="text-muted-foreground">
          Übersicht über deine Raum- und Arbeitsplatzbuchungen.
        </p>
      </div>

      {loading ? (
        <p className="text-sm text-muted-foreground">Wird geladen…</p>
      ) : (
        <div className="space-y-3">
          {bookings.map((b) => (
            <Card key={b.id}>
              <CardHeader className="flex-row items-center gap-3 space-y-0">
                {b.type === "Workplace" ? (
                  <Armchair className="size-5 text-primary" />
                ) : (
                  <DoorOpen className="size-5 text-primary" />
                )}
                <div className="min-w-0 flex-1">
                  <CardTitle className="truncate text-base">
                    {b.resource}
                  </CardTitle>
                  <p className="text-sm text-muted-foreground">
                    {b.location} · {b.date} · {b.timeRange}
                  </p>
                </div>
                <Badge variant="secondary" className="shrink-0">
                  {b.type === "Workplace" ? "Arbeitsplatz" : "Raum"}
                </Badge>
                {b.type === "ConferenceRoom" && !b.isPast && b.status === "Active" && (
                  <Button
                    variant="ghost"
                    size="icon"
                    className="shrink-0"
                    aria-label="Buchung bearbeiten"
                    onClick={() => setEditingBooking(b)}
                  >
                    <Pencil className="size-4" />
                  </Button>
                )}
              </CardHeader>
            </Card>
          ))}
        </div>
      )}

      {editingBooking && (
        <EditBookingDialog
          booking={editingBooking}
          onClose={() => setEditingBooking(null)}
          onSaved={handleSaved}
        />
      )}
    </div>
  )
}
