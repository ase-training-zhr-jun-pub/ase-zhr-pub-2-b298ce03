import { Link } from "react-router-dom"
import { Armchair, DoorOpen } from "lucide-react"
import {
  Card,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

export function Uebersicht() {
  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Willkommen bei Calvin</h1>
        <p className="text-muted-foreground">
          Mühelos den passenden Raum oder Arbeitsplatz finden und buchen – an
          jedem INNOQ-Standort.
        </p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2">
        <Link to="/raeume">
          <Card className="h-full transition-colors hover:border-primary">
            <CardHeader>
              <DoorOpen className="size-6 text-primary" />
              <CardTitle>Raum buchen</CardTitle>
              <CardDescription>
                Konferenzraum für Meeting oder Workshop reservieren.
              </CardDescription>
            </CardHeader>
          </Card>
        </Link>

        <Link to="/arbeitsplatz-buchen">
          <Card className="h-full transition-colors hover:border-primary">
            <CardHeader>
              <Armchair className="size-6 text-primary" />
              <CardTitle>Arbeitsplatz buchen</CardTitle>
              <CardDescription>
                Verfügbaren Arbeitsplatz für deinen Bürotag auswählen.
              </CardDescription>
            </CardHeader>
          </Card>
        </Link>
      </div>
    </div>
  )
}
