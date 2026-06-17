import { useEffect, useState } from "react"
import { Armchair, DoorOpen } from "lucide-react"
import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { fetchBookings, type BookingResponse } from "@/lib/api"
import { CURRENT_USER_ID } from "@/lib/current-user"

export function MeineBuchungen() {
  const [bookings, setBookings] = useState<BookingResponse[]>([])
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    fetchBookings(CURRENT_USER_ID)
      .then(setBookings)
      .catch(console.error)
      .finally(() => setLoading(false))
  }, [])

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
                <div className="min-w-0">
                  <CardTitle className="truncate text-base">
                    {b.resource}
                  </CardTitle>
                  <p className="text-sm text-muted-foreground">
                    {b.location} · {b.date} · {b.timeRange}
                  </p>
                </div>
                <Badge variant="secondary" className="ml-auto">
                  {b.type === "Workplace" ? "Arbeitsplatz" : "Raum"}
                </Badge>
              </CardHeader>
            </Card>
          ))}
        </div>
      )}
    </div>
  )
}
