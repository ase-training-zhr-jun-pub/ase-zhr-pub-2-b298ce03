import { useEffect, useState } from "react"
import { useNavigate } from "react-router-dom"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { fetchConferenceRooms, fetchLocations, type ConferenceRoom, type Location } from "@/lib/api"
import { cn } from "@/lib/utils"

function defaultDate(): string {
  const d = new Date()
  d.setDate(d.getDate() + 1)
  return d.toISOString().split("T")[0]
}

function formatDate(iso: string): string {
  const [y, m, d] = iso.split("-")
  return `${d}.${m}.${y}`
}

export function RaumBuchen() {
  const navigate = useNavigate()
  const [date, setDate] = useState(defaultDate)
  const [timeFrom, setTimeFrom] = useState("09:00")
  const [timeTo, setTimeTo] = useState("10:00")
  const [selectedRoom, setSelectedRoom] = useState<ConferenceRoom | null>(null)

  const [locations, setLocations] = useState<Location[]>([])
  const [selectedLocationId, setSelectedLocationId] = useState<string>("")
  const [rooms, setRooms] = useState<ConferenceRoom[]>([])
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    fetchLocations()
      .then((locs) => {
        setLocations(locs)
        if (locs.length > 0) setSelectedLocationId(locs[0].id)
      })
      .catch(() => setError("Standorte konnten nicht geladen werden."))
  }, [])

  useEffect(() => {
    if (!date || !timeFrom || !timeTo) return
    setLoading(true)
    setError(null)
    setSelectedRoom(null)
    fetchConferenceRooms(selectedLocationId || undefined, date, timeFrom, timeTo)
      .then((data) => {
        setRooms(data)
        setLoading(false)
      })
      .catch(() => {
        setError("Konferenzräume konnten nicht geladen werden.")
        setLoading(false)
      })
  }, [selectedLocationId, date, timeFrom, timeTo])

  const selectedLocation = locations.find((l) => l.id === selectedLocationId)

  function handleConfirm() {
    if (!selectedRoom) return
    navigate("/buchungen/neu", {
      state: {
        room: selectedRoom,
        location: selectedLocation ?? null,
        date,
        timeFrom,
        timeTo,
      },
    })
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Konferenzraum buchen</h1>
        <p className="text-muted-foreground">
          Standort und Zeitraum wählen, dann einen verfügbaren Konferenzraum auswählen.
        </p>
      </div>

      <Card>
        <CardContent className="pt-4">
          <div className="flex flex-wrap items-end gap-4">
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium" htmlFor="location">
                Standort
              </label>
              <select
                id="location"
                value={selectedLocationId}
                onChange={(e) => setSelectedLocationId(e.target.value)}
                className="h-9 rounded-md border bg-background px-3 text-sm"
              >
                <option value="">Alle Standorte</option>
                {locations.map((l) => (
                  <option key={l.id} value={l.id}>
                    {l.name}
                  </option>
                ))}
              </select>
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium" htmlFor="date">
                Datum
              </label>
              <input
                id="date"
                type="date"
                value={date}
                onChange={(e) => setDate(e.target.value)}
                className="h-9 rounded-md border bg-background px-3 text-sm"
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium" htmlFor="timeFrom">
                Von
              </label>
              <input
                id="timeFrom"
                type="time"
                value={timeFrom}
                onChange={(e) => setTimeFrom(e.target.value)}
                className="h-9 rounded-md border bg-background px-3 text-sm"
              />
            </div>
            <div className="flex flex-col gap-1.5">
              <label className="text-sm font-medium" htmlFor="timeTo">
                Bis
              </label>
              <input
                id="timeTo"
                type="time"
                value={timeTo}
                onChange={(e) => setTimeTo(e.target.value)}
                className="h-9 rounded-md border bg-background px-3 text-sm"
              />
            </div>
          </div>
        </CardContent>
      </Card>

      {error && (
        <p className="rounded-md border border-destructive/50 bg-destructive/10 px-4 py-2 text-sm text-destructive">
          {error}
        </p>
      )}

      <div className="grid grid-cols-1 gap-6 lg:grid-cols-3">
        <div className="space-y-3 lg:col-span-2">
          {loading ? (
            <p className="text-sm text-muted-foreground">Räume werden geladen…</p>
          ) : rooms.length === 0 && !error ? (
            <p className="text-sm text-muted-foreground">
              Keine Konferenzräume für diesen Zeitraum verfügbar.
            </p>
          ) : (
            rooms.map((room) => {
              const loc = locations.find((l) => l.id === room.locationId)
              const isSelected = selectedRoom?.id === room.id
              return (
                <button
                  key={room.id}
                  type="button"
                  disabled={room.occupied}
                  onClick={() => setSelectedRoom(room)}
                  className={cn(
                    "w-full rounded-lg border bg-card p-4 text-left transition-colors",
                    room.occupied
                      ? "cursor-not-allowed opacity-50"
                      : "cursor-pointer hover:border-primary/50",
                    isSelected &&
                      "border-primary ring-2 ring-primary ring-offset-2",
                  )}
                >
                  <div className="flex items-start justify-between gap-2">
                    <div>
                      <p className="font-semibold">{room.name}</p>
                      <p className="text-sm text-muted-foreground">
                        {loc?.name ?? selectedLocation?.name} · {room.capacity} Personen
                      </p>
                    </div>
                    {room.occupied ? (
                      <Badge variant="secondary">Belegt</Badge>
                    ) : (
                      <Badge variant="outline" className="border-green-600 text-green-700">
                        Verfügbar
                      </Badge>
                    )}
                  </div>
                  <div className="mt-2 flex flex-wrap gap-1.5">
                    {room.equipment.map((e) => (
                      <span
                        key={e}
                        className="rounded-full bg-muted px-2 py-0.5 text-xs text-muted-foreground"
                      >
                        {e}
                      </span>
                    ))}
                  </div>
                </button>
              )
            })
          )}
        </div>

        <div className="lg:col-span-1">
          {selectedRoom ? (
            <Card className="sticky top-20">
              <CardHeader>
                <CardTitle className="text-base">Ausgewählter Raum</CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div>
                  <p className="text-xl font-semibold">{selectedRoom.name}</p>
                  <p className="text-sm text-muted-foreground">
                    {locations.find((l) => l.id === selectedRoom.locationId)?.name ??
                      selectedLocation?.name}
                  </p>
                </div>

                <Separator />

                <div className="space-y-2 text-sm">
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Kapazität</span>
                    <span>{selectedRoom.capacity} Personen</span>
                  </div>
                  <div>
                    <span className="text-muted-foreground">Ausstattung</span>
                    <div className="mt-1 flex flex-wrap gap-1">
                      {selectedRoom.equipment.map((e) => (
                        <span
                          key={e}
                          className="rounded-full bg-muted px-2 py-0.5 text-xs"
                        >
                          {e}
                        </span>
                      ))}
                    </div>
                  </div>
                </div>

                <Separator />

                <div className="text-sm">
                  <p className="text-muted-foreground">Zeitraum</p>
                  <p className="font-medium">{formatDate(date)}</p>
                  <p>
                    {timeFrom} – {timeTo} Uhr
                  </p>
                </div>

                <Button className="w-full" onClick={handleConfirm}>
                  Raum bestätigen
                </Button>
              </CardContent>
            </Card>
          ) : (
            <Card className="border-dashed">
              <CardContent className="flex min-h-40 items-center justify-center text-center text-sm text-muted-foreground">
                Konferenzraum auswählen, um Details anzuzeigen.
              </CardContent>
            </Card>
          )}
        </div>
      </div>
    </div>
  )
}
