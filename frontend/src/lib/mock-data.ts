// Zentrale Mock-Daten für den Calvin-Prototyp. Kein Backend — alle Daten gemockt.

export type ConferenceRoom = {
  id: string
  name: string
  capacity: number
  equipment: string[]
  occupied: boolean
  locationId: string
}

export const conferenceRooms: ConferenceRoom[] = [
  {
    id: "KR-KOE-01",
    name: "Rheinblick",
    capacity: 8,
    equipment: ["Bildschirm", "Whiteboard", "Videokonferenz"],
    occupied: false,
    locationId: "koeln",
  },
  {
    id: "KR-KOE-02",
    name: "Stadtgarten",
    capacity: 4,
    equipment: ["Bildschirm", "Flipchart"],
    occupied: true,
    locationId: "koeln",
  },
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
    equipment: ["Bildschirm", "Flipchart", "Whiteboard"],
    occupied: false,
    locationId: "berlin",
  },
  {
    id: "KR-HAM-01",
    name: "Speicherstadt",
    capacity: 10,
    equipment: ["Bildschirm", "Videokonferenz"],
    occupied: true,
    locationId: "hamburg",
  },
  {
    id: "KR-MUC-01",
    name: "Isar",
    capacity: 8,
    equipment: ["Bildschirm", "Whiteboard"],
    occupied: false,
    locationId: "muenchen",
  },
]

export type Standort = {
  id: string
  name: string
}

export const standorte: Standort[] = [
  { id: "koeln", name: "Köln" },
  { id: "berlin", name: "Berlin" },
  { id: "hamburg", name: "Hamburg" },
  { id: "monheim", name: "Monheim" },
  { id: "muenchen", name: "München" },
  { id: "offenbach", name: "Offenbach" },
  { id: "zuerich", name: "Zürich" },
  { id: "baar", name: "Baar" },
]

export type Arbeitsplatz = {
  id: string
  bezeichnung: string
  etage: string
  ausstattung: string[]
  belegt: boolean
  standortId: string
}

export const arbeitsplaetze: Arbeitsplatz[] = [
  {
    id: "AP-01",
    bezeichnung: "Fensterplatz Nord",
    etage: "Etage 1",
    ausstattung: ["Höhenverstellbarer Tisch", "Fensterplatz"],
    belegt: false,
    standortId: "koeln",
  },
  {
    id: "AP-02",
    bezeichnung: "Teambereich",
    etage: "Etage 1",
    ausstattung: ["Dockingstation"],
    belegt: true,
    standortId: "koeln",
  },
  {
    id: "AP-03",
    bezeichnung: "Ruhezone",
    etage: "Etage 2",
    ausstattung: ["2 Monitore", "Höhenverstellbarer Tisch"],
    belegt: false,
    standortId: "koeln",
  },
  {
    id: "AP-04",
    bezeichnung: "Fokusplatz",
    etage: "Etage 2",
    ausstattung: ["2 Monitore", "Dockingstation"],
    belegt: false,
    standortId: "koeln",
  },
  {
    id: "AP-05",
    bezeichnung: "Gemeinschaftstisch",
    etage: "Etage 2",
    ausstattung: ["Großer Bildschirm"],
    belegt: true,
    standortId: "koeln",
  },
  {
    id: "AP-06",
    bezeichnung: "Einzelplatz Süd",
    etage: "Etage 3",
    ausstattung: ["Höhenverstellbarer Tisch", "Dockingstation"],
    belegt: false,
    standortId: "koeln",
  },
]

export type Buchung = {
  id: string
  typ: "Raum" | "Arbeitsplatz"
  ressource: string
  standort: string
  datum: string
  zeitraum: string
}

export const meineBuchungen: Buchung[] = [
  {
    id: "CLVN-B-1001",
    typ: "Arbeitsplatz",
    ressource: "AP-03 · Ruhezone",
    standort: "Köln",
    datum: "18.06.2026",
    zeitraum: "Ganztägig",
  },
  {
    id: "CLVN-B-1002",
    typ: "Raum",
    ressource: "Konferenzraum Rheinblick",
    standort: "Köln",
    datum: "19.06.2026",
    zeitraum: "10:00 – 11:30",
  },
]
