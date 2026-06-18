import { useEffect, useState } from "react"
import { ArrowRight, Check, MapPin } from "lucide-react"
import {
  Card,
  CardContent,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { cn } from "@/lib/utils"
import {
  fetchLocations,
  fetchWorkplaces,
  createBooking,
  type Location,
  type Workplace,
} from "@/lib/api"
import { CURRENT_USER_ID } from "@/lib/current-user"

export function ArbeitsplatzBuchen() {
  const [locations, setLocations] = useState<Location[]>([])
  const [workplaces, setWorkplaces] = useState<Workplace[]>([])
  const [loadingWorkplaces, setLoadingWorkplaces] = useState(false)
  const [standortId, setStandortId] = useState("koeln")
  const [datum, setDatum] = useState(new Date().toISOString().slice(0, 10))
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [booking, setBooking] = useState<"idle" | "loading" | "done" | "error">("idle")

  useEffect(() => {
    fetchLocations().then(setLocations).catch(console.error)
  }, [])

  useEffect(() => {
    setLoadingWorkplaces(true)
    fetchWorkplaces(standortId, datum)
      .then(setWorkplaces)
      .catch(console.error)
      .finally(() => setLoadingWorkplaces(false))
  }, [standortId, datum])

  const selected = workplaces.find((w) => w.id === selectedId) ?? null

  function handleSelect(id: string) {
    setSelectedId((current) => (current === id ? null : id))
    setBooking("idle")
  }

  async function handleConfirm() {
    if (!selected) return
    setBooking("loading")
    try {
      await createBooking({
        type: "Workplace",
        resourceId: selected.id,
        locationId: standortId,
        userId: CURRENT_USER_ID,
        date: datum,
      })
      setBooking("done")
    } catch {
      setBooking("error")
    }
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Arbeitsplatz buchen</h1>
        <p className="text-muted-foreground">
          Schritt 1 von 3 · Arbeitsplatz auswählen
        </p>
      </div>

      <Card>
        <CardContent className="flex flex-wrap items-end gap-4 pt-6">
          <div className="space-y-1.5">
            <label className="text-sm font-medium">Standort</label>
            <Select
              value={standortId}
              onValueChange={(v) => {
                if (!v) return
                setStandortId(v)
                setSelectedId(null)
                setBooking("idle")
              }}
            >
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Standort" />
              </SelectTrigger>
              <SelectContent>
                {locations.map((l) => (
                  <SelectItem key={l.id} value={l.id}>
                    {l.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>

          <div className="space-y-1.5">
            <label className="text-sm font-medium" htmlFor="datum">
              Datum
            </label>
            <input
              id="datum"
              type="date"
              value={datum}
              onChange={(e) => {
                setDatum(e.target.value)
                setSelectedId(null)
                setBooking("idle")
              }}
              className="h-9 rounded-lg border border-input bg-transparent px-3 text-sm outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50"
            />
          </div>
        </CardContent>
      </Card>

      <div>
        <h2 className="mb-3 text-sm font-semibold text-muted-foreground">
          Verfügbare Arbeitsplätze
        </h2>
        {loadingWorkplaces ? (
          <p className="text-sm text-muted-foreground">Wird geladen…</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {workplaces.map((w) => {
              const isSelected = w.id === selectedId
              return (
                <Card
                  key={w.id}
                  className={cn(
                    "transition-colors",
                    w.occupied && "opacity-60",
                    isSelected && "border-primary ring-1 ring-primary",
                  )}
                >
                  <CardHeader>
                    <div className="flex items-center justify-between gap-2">
                      <CardTitle className="text-base">{w.id}</CardTitle>
                      {w.occupied ? (
                        <Badge variant="destructive">belegt</Badge>
                      ) : (
                        <Badge className="bg-emerald-600 text-white">frei</Badge>
                      )}
                    </div>
                    <p className="text-sm text-muted-foreground">{w.name}</p>
                  </CardHeader>
                  <CardContent className="space-y-2 text-sm">
                    <p className="flex items-center gap-1.5 text-muted-foreground">
                      <MapPin className="size-3.5" /> {w.floor}
                    </p>
                    <div className="flex flex-wrap gap-1.5">
                      {w.equipment.map((e) => (
                        <Badge key={e} variant="outline">
                          {e}
                        </Badge>
                      ))}
                    </div>
                  </CardContent>
                  <CardFooter>
                    <Button
                      variant={isSelected ? "default" : "outline"}
                      className="w-full"
                      disabled={w.occupied}
                      onClick={() => handleSelect(w.id)}
                    >
                      {w.occupied ? (
                        "Belegt"
                      ) : isSelected ? (
                        <>
                          <Check className="size-4" /> Ausgewählt
                        </>
                      ) : (
                        "Auswählen"
                      )}
                    </Button>
                  </CardFooter>
                </Card>
              )
            })}
          </div>
        )}
      </div>

      <div className="sticky bottom-0 -mx-4 border-t bg-card/95 px-4 py-3 backdrop-blur md:-mx-8 md:px-8">
        <div className="mx-auto flex max-w-5xl flex-wrap items-center gap-3">
          <div className="min-w-0 text-sm">
            {selected ? (
              <span>
                Auswahl:{" "}
                <span className="font-medium">
                  {selected.id} · {selected.name}
                </span>{" "}
                <span className="text-muted-foreground">
                  ({selected.equipment.join(", ")} · {selected.floor})
                </span>
              </span>
            ) : (
              <span className="text-muted-foreground">
                Noch kein Arbeitsplatz ausgewählt
              </span>
            )}
          </div>
          <Button
            className="ml-auto"
            disabled={!selected || booking === "loading" || booking === "done"}
            onClick={handleConfirm}
          >
            {booking === "loading" ? (
              "Wird gebucht…"
            ) : (
              <>
                Bestätigen <ArrowRight className="size-4" />
              </>
            )}
          </Button>
        </div>
        {booking === "done" && selected && (
          <p className="mx-auto mt-2 flex max-w-5xl items-center gap-1.5 text-sm text-emerald-700">
            <Check className="size-4" /> {selected.id} erfolgreich gebucht.
          </p>
        )}
        {booking === "error" && (
          <p className="mx-auto mt-2 max-w-5xl text-sm text-destructive">
            Buchung fehlgeschlagen. Bitte erneut versuchen.
          </p>
        )}
      </div>
    </div>
  )
}
