import { useMemo, useState } from "react"
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
import { arbeitsplaetze, standorte } from "@/lib/mock-data"
import { cn } from "@/lib/utils"

export function ArbeitsplatzBuchen() {
  const [standortId, setStandortId] = useState("koeln")
  const [datum, setDatum] = useState("2026-06-17")
  const [selectedId, setSelectedId] = useState<string | null>(null)
  const [confirmed, setConfirmed] = useState(false)

  const plaetze = useMemo(
    () => arbeitsplaetze.filter((a) => a.standortId === standortId),
    [standortId],
  )

  const selected = plaetze.find((a) => a.id === selectedId) ?? null

  function handleSelect(id: string) {
    setSelectedId((current) => (current === id ? null : id))
  }

  return (
    <div className="mx-auto max-w-5xl space-y-6">
      {/* Kopf + Schrittanzeige */}
      <div>
        <h1 className="text-2xl font-semibold">Arbeitsplatz buchen</h1>
        <p className="text-muted-foreground">
          Schritt 1 von 3 · Arbeitsplatz auswählen
        </p>
      </div>

      {/* Filter: Standort + Datum */}
      <Card>
        <CardContent className="flex flex-wrap items-end gap-4 pt-6">
          <div className="space-y-1.5">
            <label className="text-sm font-medium">Standort</label>
            <Select
              items={standorte.map((s) => ({ value: s.id, label: s.name }))}
              value={standortId}
              onValueChange={(v) => {
                if (!v) return
                setStandortId(v)
                setSelectedId(null)
                setConfirmed(false)
              }}
            >
              <SelectTrigger className="w-[180px]">
                <SelectValue placeholder="Standort" />
              </SelectTrigger>
              <SelectContent>
                {standorte.map((s) => (
                  <SelectItem key={s.id} value={s.id}>
                    {s.name}
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
                setConfirmed(false)
              }}
              className="h-9 rounded-lg border border-input bg-transparent px-3 text-sm outline-none focus-visible:border-ring focus-visible:ring-[3px] focus-visible:ring-ring/50"
            />
          </div>
        </CardContent>
      </Card>

      {/* Arbeitsplätze als Karten-Raster */}
      <div>
        <h2 className="mb-3 text-sm font-semibold text-muted-foreground">
          Verfügbare Arbeitsplätze
        </h2>
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {plaetze.map((a) => {
            const isSelected = a.id === selectedId
            return (
              <Card
                key={a.id}
                className={cn(
                  "transition-colors",
                  a.belegt && "opacity-60",
                  isSelected && "border-primary ring-1 ring-primary",
                )}
              >
                <CardHeader>
                  <div className="flex items-center justify-between gap-2">
                    <CardTitle className="text-base">{a.id}</CardTitle>
                    {a.belegt ? (
                      <Badge variant="destructive">belegt</Badge>
                    ) : (
                      <Badge className="bg-emerald-600 text-white">frei</Badge>
                    )}
                  </div>
                  <p className="text-sm text-muted-foreground">
                    {a.bezeichnung}
                  </p>
                </CardHeader>
                <CardContent className="space-y-2 text-sm">
                  <p className="flex items-center gap-1.5 text-muted-foreground">
                    <MapPin className="size-3.5" /> {a.etage}
                  </p>
                  <div className="flex flex-wrap gap-1.5">
                    {a.ausstattung.map((aus) => (
                      <Badge key={aus} variant="outline">
                        {aus}
                      </Badge>
                    ))}
                  </div>
                </CardContent>
                <CardFooter>
                  <Button
                    variant={isSelected ? "default" : "outline"}
                    className="w-full"
                    disabled={a.belegt}
                    onClick={() => handleSelect(a.id)}
                  >
                    {a.belegt ? (
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
      </div>

      {/* Auswahl-Leiste + Bestätigen */}
      <div className="sticky bottom-0 -mx-4 border-t bg-card/95 px-4 py-3 backdrop-blur md:-mx-8 md:px-8">
        <div className="mx-auto flex max-w-5xl flex-wrap items-center gap-3">
          <div className="min-w-0 text-sm">
            {selected ? (
              <span>
                Auswahl:{" "}
                <span className="font-medium">
                  {selected.id} · {selected.bezeichnung}
                </span>{" "}
                <span className="text-muted-foreground">
                  ({selected.ausstattung.join(", ")} · {selected.etage})
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
            disabled={!selected}
            onClick={() => setConfirmed(true)}
          >
            Bestätigen <ArrowRight className="size-4" />
          </Button>
        </div>
        {confirmed && selected && (
          <p className="mx-auto mt-2 flex max-w-5xl items-center gap-1.5 text-sm text-emerald-700">
            <Check className="size-4" /> {selected.id} bestätigt – weiter zu
            Schritt 2 (Buchung absenden).
          </p>
        )}
      </div>
    </div>
  )
}
