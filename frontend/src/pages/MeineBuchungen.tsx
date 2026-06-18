import { useEffect, useState } from "react"
import { Armchair, DoorOpen, Pencil } from "lucide-react"
import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/components/ui/alert-dialog"
import { fetchBookings, cancelBooking, type BookingResponse } from "@/lib/api"
import { CURRENT_USER_ID } from "@/lib/current-user"
import { EditBookingDialog } from "@/components/EditBookingDialog"

export function MeineBuchungen() {
  const [bookings, setBookings] = useState<BookingResponse[]>([])
  const [loading, setLoading] = useState(true)
  const [cancellingId, setCancellingId] = useState<string | null>(null)
  const [successMessage, setSuccessMessage] = useState<string | null>(null)
  const [errorMessage, setErrorMessage] = useState<string | null>(null)
  const [editingBooking, setEditingBooking] = useState<BookingResponse | null>(null)

  useEffect(() => {
    fetchBookings(CURRENT_USER_ID)
      .then(setBookings)
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

  async function handleCancel(id: string) {
    setCancellingId(id)
    setSuccessMessage(null)
    setErrorMessage(null)
    try {
      await cancelBooking(id)
      setBookings((prev) => prev.filter((b) => b.id !== id))
      setSuccessMessage("Buchung wurde erfolgreich storniert.")
    } catch (err) {
      setErrorMessage(err instanceof Error ? err.message : "Stornierung fehlgeschlagen.")
    } finally {
      setCancellingId(null)
    }
  }

  function handleSaved(updated: BookingResponse) {
    setBookings((prev) => prev.map((b) => (b.id === updated.id ? updated : b)))
    setEditingBooking(null)
  }

  const activeBookings = bookings.filter((b) => b.status !== "Cancelled")


  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Meine Buchungen</h1>
        <p className="text-muted-foreground">
          Übersicht über deine Raum- und Arbeitsplatzbuchungen.
        </p>
      </div>

      {successMessage && (
        <div className="rounded-md border border-green-200 bg-green-50 px-4 py-3 text-sm text-green-800">
          {successMessage}
        </div>
      )}

      {errorMessage && (
        <div className="rounded-md border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-800">
          {errorMessage}
        </div>
      )}

      {loading ? (
        <p className="text-sm text-muted-foreground">Wird geladen…</p>
      ) : activeBookings.length === 0 ? (
        <p className="text-sm text-muted-foreground">Keine aktiven Buchungen vorhanden.</p>
      ) : (
        <div className="space-y-3">
          {activeBookings.map((b) => (
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
                {!b.isPast && (
                  <AlertDialog>
                    <AlertDialogTrigger
                      render={
                        <Button
                          variant="outline"
                          size="sm"
                          disabled={cancellingId === b.id}
                        >
                          {cancellingId === b.id ? "Wird storniert…" : "Stornieren"}
                        </Button>
                      }
                    />
                    <AlertDialogContent>
                      <AlertDialogHeader>
                        <AlertDialogTitle>Buchung stornieren?</AlertDialogTitle>
                        <AlertDialogDescription>
                          Möchtest du die Buchung für <strong>{b.resource}</strong> am{" "}
                          <strong>{b.date}</strong> wirklich stornieren? Diese Aktion kann nicht
                          rückgängig gemacht werden.
                        </AlertDialogDescription>
                      </AlertDialogHeader>
                      <AlertDialogFooter>
                        <AlertDialogCancel>Abbrechen</AlertDialogCancel>
                        <AlertDialogAction onClick={() => handleCancel(b.id)}>
                          Stornieren
                        </AlertDialogAction>
                      </AlertDialogFooter>
                    </AlertDialogContent>
                  </AlertDialog>
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
