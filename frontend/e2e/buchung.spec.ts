import { test, expect, type Route } from "@playwright/test"

// ── Mock-Daten ────────────────────────────────────────────────────────────────

const INITIAL_BOOKINGS = [
  {
    id: "CLVN-B-1002",
    type: "ConferenceRoom",
    resource: "Rheinblick",
    location: "Köln",
    date: "19.06.2026",
    timeRange: "10:00 – 11:30",
    resourceId: "KR-KOE-01",
    locationId: "koeln",
    userId: "alice",
    dateIso: "2026-06-19",
    timeFrom: "10:00",
    timeTo: "11:30",
    title: "Team-Sync",
    notes: null,
    status: "Active",
    isPast: false,
    equipment: ["Bildschirm", "Whiteboard", "Videokonferenz"],
  },
]

const NEW_BOOKING = {
  id: "CLVN-B-9999",
  type: "ConferenceRoom",
  resource: "Spreebogen",
  location: "Berlin",
  date: "25.06.2026",
  timeRange: "14:00 – 15:00",
  resourceId: "KR-BER-01",
  locationId: "berlin",
  userId: "alice",
  dateIso: "2026-06-25",
  timeFrom: "14:00",
  timeTo: "15:00",
  title: "",
  notes: null,
  status: "Active",
  isPast: false,
  equipment: ["Bildschirm", "Whiteboard", "Videokonferenz"],
}

const LOCATIONS = [
  { id: "koeln", name: "Köln" },
  { id: "berlin", name: "Berlin" },
]

const ROOMS_BERLIN = [
  {
    id: "KR-BER-01",
    name: "Spreebogen",
    capacity: 12,
    equipment: ["Bildschirm", "Whiteboard", "Videokonferenz"],
    occupied: false,
    locationId: "berlin",
  },
  {
    id: "KR-BER-02",
    name: "Brandenburger",
    capacity: 6,
    equipment: ["Bildschirm", "Flipchart"],
    occupied: true,
    locationId: "berlin",
  },
]

// ── Hilfsfunktion ─────────────────────────────────────────────────────────────

/** Registriert alle API-Mocks. bookingCreated steuert, ob die Buchungsliste
 *  schon die neue Buchung enthält. */
function setupApiMocks(bookingCreated: { value: boolean }) {
  return async (route: Route) => {
    const url = route.request().url()
    const method = route.request().method()

    if (url.includes("/api/bookings") && !url.includes("/cancellation")) {
      if (method === "GET") {
        const bookings = bookingCreated.value
          ? [...INITIAL_BOOKINGS, NEW_BOOKING]
          : INITIAL_BOOKINGS
        await route.fulfill({ json: bookings })
      } else if (method === "POST") {
        bookingCreated.value = true
        await route.fulfill({ status: 201, json: NEW_BOOKING })
      } else {
        await route.continue()
      }
    } else if (url.includes("/api/locations")) {
      await route.fulfill({ json: LOCATIONS })
    } else if (url.includes("/api/conference-rooms")) {
      await route.fulfill({ json: ROOMS_BERLIN })
    } else {
      await route.continue()
    }
  }
}

// ── Test ──────────────────────────────────────────────────────────────────────

test("Raumbuchungsprozess: neue Buchung anlegen und in Übersicht verifizieren", async ({ page }) => {
  const bookingCreated = { value: false }
  await page.route("**/api/**", setupApiMocks(bookingCreated))

  // ── Schritt 1: Buchungsübersicht öffnen ──────────────────────────────────────
  await page.goto("/meine-buchungen")
  await expect(page.getByRole("heading", { name: "Meine Buchungen" })).toBeVisible()

  // ── Schritt 2: Bisherige Buchungen merken ────────────────────────────────────
  await expect(page.getByText("Rheinblick")).toBeVisible()
  const bookingCardsBefore = await page.getByTestId("booking-item").count()
  expect(bookingCardsBefore).toBe(1)
  await expect(page.getByText("Spreebogen")).not.toBeVisible()

  // ── Schritt 3: Standort-Seite öffnen ────────────────────────────────────────
  await page.getByRole("link", { name: "Raum buchen" }).click()
  await expect(page.getByRole("heading", { name: "Konferenzraum buchen" })).toBeVisible()

  // ── Schritt 4: Standort auswählen ────────────────────────────────────────────
  await page.selectOption("#location", "berlin")

  // ── Schritt 5: Datum auswählen ───────────────────────────────────────────────
  await page.fill("#date", "2026-06-25")

  // ── Schritt 6: Raum auswählen ────────────────────────────────────────────────
  // Warten bis Räume geladen sind
  await expect(page.getByText("Spreebogen")).toBeVisible()
  // Belegter Raum ist nicht wählbar
  await expect(page.getByText("Brandenburger").locator("..").locator("..")).toContainText("Belegt")

  // Verfügbaren Raum anklicken
  await page.getByRole("button", { name: /Spreebogen/ }).click()

  // Sidebar zeigt Raumdetails
  await expect(page.getByText("Ausgewählter Raum")).toBeVisible()

  // ── Schritt 7: Buchungsprozess starten ───────────────────────────────────────
  // "Raum bestätigen" → navigiert zu /buchungen/neu
  await page.getByRole("button", { name: "Raum bestätigen" }).click()
  await expect(page.getByRole("heading", { name: "Buchungsdetails" })).toBeVisible()

  // Buchungsdetails prüfen
  await expect(page.getByText("Spreebogen")).toBeVisible()
  await expect(page.getByText("25.06.2026")).toBeVisible()

  // "Buchung absenden" → POST /api/bookings → navigiert zu /meine-buchungen
  await page.getByRole("button", { name: "Buchung absenden" }).click()

  // ── Schritt 8: Buchungsübersicht öffnen (nach Weiterleitung) ─────────────────
  await expect(page).toHaveURL(/meine-buchungen/)
  await expect(page.getByRole("heading", { name: "Meine Buchungen" })).toBeVisible()

  // ── Schritt 9: Neue Buchung verifizieren ─────────────────────────────────────
  // Jetzt müssen beide Buchungen sichtbar sein
  await expect(page.getByText("Rheinblick")).toBeVisible()
  await expect(page.getByText("Spreebogen")).toBeVisible()

  // Gesamtanzahl hat sich um 1 erhöht
  const bookingCardsAfter = await page.getByTestId("booking-item").count()
  expect(bookingCardsAfter).toBe(bookingCardsBefore + 1)
})
