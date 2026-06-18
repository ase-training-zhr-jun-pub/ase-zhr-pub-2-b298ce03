import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

export function BookingDetails() {
  return (
    <div className="mx-auto max-w-2xl space-y-6">
      <div>
        <h1 className="text-2xl font-semibold">Buchungsdetails</h1>
        <p className="text-muted-foreground">
          Meetingtitel und weitere Details eingeben.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>In Arbeit</CardTitle>
        </CardHeader>
        <CardContent className="text-sm text-muted-foreground">
          Buchungsdetails werden im Rahmen von CLVN-017 und CLVN-018 umgesetzt.
        </CardContent>
      </Card>
    </div>
  )
}
