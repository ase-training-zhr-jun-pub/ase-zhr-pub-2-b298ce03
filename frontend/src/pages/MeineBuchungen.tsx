import { Armchair, DoorOpen } from "lucide-react"
import { Card, CardHeader, CardTitle } from "@/components/ui/card"
import { Badge } from "@/components/ui/badge"
import { meineBuchungen } from "@/lib/mock-data"

export function MeineBuchungen() {
  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Meine Buchungen</h1>
        <p className="text-muted-foreground">
          Übersicht über deine Raum- und Arbeitsplatzbuchungen.
        </p>
      </div>

      <div className="space-y-3">
        {meineBuchungen.map((b) => (
          <Card key={b.id}>
            <CardHeader className="flex-row items-center gap-3 space-y-0">
              {b.typ === "Arbeitsplatz" ? (
                <Armchair className="size-5 text-primary" />
              ) : (
                <DoorOpen className="size-5 text-primary" />
              )}
              <div className="min-w-0">
                <CardTitle className="truncate text-base">
                  {b.ressource}
                </CardTitle>
                <p className="text-sm text-muted-foreground">
                  {b.standort} · {b.datum} · {b.zeitraum}
                </p>
              </div>
              <Badge variant="secondary" className="ml-auto">
                {b.typ}
              </Badge>
            </CardHeader>
          </Card>
        ))}
      </div>
    </div>
  )
}
