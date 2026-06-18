import { useEffect, useState } from "react"
import { useLocation, useNavigate } from "react-router-dom"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { Button } from "@/components/ui/button"
import { createBooking, type ConferenceRoom, type Location } from "@/lib/api"
import { CURRENT_USER_ID } from "@/lib/current-user"

type BookingState = {
  room: ConferenceRoom
  location: Location | null
  date: string
  timeFrom: string
  timeTo: string
}

function formatDate(iso: string): string {
  const [y, m, d] = iso.split("-")
  return `${d}.${m}.${y}`
}

export function BookingDetails() {
  const location = useLocation()
  const navigate = useNavigate()
  const state = location.state as BookingState | null
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState<string | null>(null)

  useEffect(() => {
    if (!state?.room) {
      navigate("/raeume", { replace: true })
    }
  }, [state, navigate])

  if (!state?.room) return null

  const { room, location: loc, date, timeFrom, timeTo } = state

  async function handleSubmit() {
    setSubmitting(true)
    setSubmitError(null)
    try {
      await createBooking({
        type: "ConferenceRoom",
        resourceId: room.id,
        locationId: loc?.id ?? "",
        userId: CURRENT_USER_ID,
        date,
        timeFrom,
        timeTo,
      })
      navigate("/meine-buchungen")
    } catch (err) {
      setSubmitError(err instanceof Error ? err.message : "Buchung konnte nicht gespeichert werden.")
      setSubmitting(false)
    }
  }

  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Buchungsdetails</h1>
        <p className="text-muted-foreground">
          Bitte prüfe deine Raumauswahl und gib weitere Details ein.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Ausgewählter Raum</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex items-start justify-between gap-2">
            <div>
              <p className="text-lg font-semibold">{room.name}</p>
              <p className="text-sm text-muted-foreground">
                {loc?.name} · {room.capacity} Personen
              </p>
            </div>
            <Badge variant="outline" className="border-green-600 text-green-700">
              Verfügbar
            </Badge>
          </div>

          <div className="flex flex-wrap gap-1.5">
            {room.equipment.map((e) => (
              <span
                key={e}
                className="rounded-full bg-muted px-2 py-0.5 text-xs text-muted-foreground"
              >
                {e}
              </span>
            ))}
          </div>

          <Separator />

          <div className="text-sm">
            <p className="text-muted-foreground">Zeitraum</p>
            <p className="font-medium">{formatDate(date)}</p>
            <p>
              {timeFrom} – {timeTo} Uhr
            </p>
          </div>
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle className="text-base">Weitere Details</CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          Meetingtitel und Buchungsnotiz werden im Rahmen von CLVN-017 und CLVN-018 umgesetzt.
        </CardContent>
      </Card>

      {submitError && (
        <div className="rounded-md border border-destructive/50 bg-destructive/10 px-4 py-3 text-sm text-destructive">
          {submitError}
        </div>
      )}

      <div className="flex gap-3">
        <Button variant="outline" onClick={() => navigate(-1)} disabled={submitting}>
          Zurück
        </Button>
        <Button className="flex-1" onClick={handleSubmit} disabled={submitting}>
          {submitting ? "Wird gespeichert…" : "Buchung absenden"}
        </Button>
      </div>
    </div>
  )
}
