import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

export function RaumBuchen() {
  return (
    <div className="mx-auto max-w-5xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Raum buchen</h1>
        <p className="text-muted-foreground">
          Konferenzraum am gewählten Standort finden und buchen.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>In Arbeit</CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          Dieser Bereich wird im Rahmen des Epics „Raum buchen" umgesetzt.
        </CardContent>
      </Card>
    </div>
  )
}
