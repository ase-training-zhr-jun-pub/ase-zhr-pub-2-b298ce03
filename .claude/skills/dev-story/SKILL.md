---
name: dev-story
description: Entwickelt eine User Story vollständig: Planung, Implementierung, Verifikation. Input ist die Story-Nummer (z.B. 016).
argument-hint: "<story-nummer> (z.B. 016)"
---

<role>
Du bist ein erfahrener Fullstack-Entwickler für das Calvin Raumbuchungssystem (React/TypeScript Frontend + ASP.NET Minimal API Backend). Du implementierst User Stories präzise nach ihren Akzeptanzkriterien — kein Gold-Plating, kein Scope Creep.
</role>

<context>
## Architektur
@CLAUDE.md

## Glossar (Ubiquitous Language)
@docs/produkt/glossar.md

## Bestehende Frontend-Seiten
- `frontend/src/pages/Uebersicht.tsx` — Startseite
- `frontend/src/pages/RaumBuchen.tsx` — Raum buchen (Suche, Auswahl, Buchungsformular)
- `frontend/src/pages/ArbeitsplatzBuchen.tsx` — Arbeitsplatz buchen
- `frontend/src/pages/MeineBuchungen.tsx` — Buchungsübersicht

## Mock-Daten
- `frontend/src/lib/mock-data.ts` — alle Frontend-Mock-Daten

## Backend-Struktur
- `backend/Domain/` — C# Records (Domänenobjekte)
- `backend/Data/InMemoryStore.cs` — In-Memory Datenspeicher
- `backend/Endpoints/` — Minimal API Endpoints (eine Klasse pro Ressource)
- `backend/Http/` — Request/Response DTOs
</context>

<instructions>
Führe diese Phasen **strikt nacheinander** aus. Warte nach Phase 1 und Phase 2 auf die **explizite Freigabe** des Nutzers, bevor du fortfährst.

---

## Phase 1: Analyse & Implementierungsplan

### 1.1 Story lesen
- Bestimme den Dateipfad: `docs/produkt/backlog/CLVN-$STORY_NR-STORY-*.md`
  - Suche die Datei mit: `find docs/produkt/backlog -name "CLVN-$STORY_NR-STORY-*.md"`
- Lies die Story vollständig (User Story Satz, Beschreibung, Akzeptanzkriterien)

### 1.2 Aktuellen Code analysieren
Lese die relevanten Dateien, die voraussichtlich betroffen sind:
- Betroffene Frontend-Seiten und Komponenten
- Betroffene Backend-Endpoints und DTOs
- Mock-Daten in `frontend/src/lib/mock-data.ts`
- Routing in `frontend/src/App.tsx`

### 1.3 Konfliktcheck
Prüfe ob die Story im Widerspruch zur aktuellen Implementierung steht:
- Gibt es existierende Felder/Routes/Typen, die der Story widersprechen?
- Macht die Story Annahmen über Flows, die anders implementiert sind?

Falls Konflikte gefunden: Beschreibe sie klar und frage den Nutzer, wie vorzugehen ist. Passe das Story-Verständnis entsprechend an.

### 1.4 Implementierungsplan erstellen

Präsentiere den Plan in diesen Abschnitten:

#### A) Betroffene Bereiche (High-Level)
Tabellarische Übersicht:
| Bereich | Datei/Komponente | Art der Änderung |
|---------|-----------------|-----------------|
| Frontend Page | ... | Neu / Geändert / Unverändert |
| Frontend Komponente | ... | ... |
| Backend Endpoint | ... | ... |
| Backend DTO | ... | ... |
| Mock-Daten | ... | ... |

#### B) API-Änderungen (Detail)
Für jeden neuen oder geänderten Endpoint:
- HTTP-Methode + Route (z.B. `POST /api/bookings`)
- Request DTO: Feldnamen + Typen
- Response DTO: Feldnamen + Typen

#### C) Frontend-Routen (Detail)
Für neue oder geänderte Routen:
- Route-Pfad (z.B. `/buchen/bestaetigung`)
- Komponenten-Datei (z.B. `frontend/src/pages/BuchungsBestaetigung.tsx`)

#### D) Neue Typen/Interfaces (Detail)
TypeScript-Interfaces oder C# Records, die neu erstellt werden — mit genauen Property-Namen.

#### E) Was NICHT implementiert wird
Explizite Liste von Dingen, die nahe liegen könnten, aber **nicht** in der Story stehen.

---

**STOPP — Warte auf Freigabe durch den Nutzer, bevor du mit Phase 2 beginnst.**

---

## Phase 2: Implementierung

Implementiere **nur** was im genehmigten Plan steht. Reihenfolge:

1. **Backend** (falls Änderungen nötig):
   - Domain Records in `backend/Domain/`
   - DTOs in `backend/Http/`
   - Endpoint-Erweiterungen in `backend/Endpoints/`
   - `InMemoryStore`-Methoden in `backend/Data/`

2. **Frontend Mock-Daten** (falls nötig):
   - `frontend/src/lib/mock-data.ts` anpassen

3. **Frontend Komponenten/Seiten**:
   - Neue oder geänderte Seiten in `frontend/src/pages/`
   - Neue oder geänderte Komponenten in `frontend/src/components/`

4. **Routing**:
   - `frontend/src/App.tsx` falls neue Routen

### Implementierungsregeln
- Keine Kommentare außer wenn das Warum nicht offensichtlich ist
- Keine Features, die nicht explizit in der Story stehen
- Kein Error-Handling für Szenarien, die nicht auftreten können
- Deutsche UI-Texte, englische Code-Namen (Klassen, Variablen, Routen)
- Fachbegriffe aus dem Glossar verwenden
- Relative API-Pfade ohne führenden Slash: `fetch("api/bookings")`

---

## Phase 3: Verifikation

Führe diese Schritte nacheinander aus:

### 3.1 Backend Build
```bash
cd backend && dotnet build
```
Bei Fehlern: Sofort beheben, dann erneut prüfen.

### 3.2 Frontend Build
```bash
cd frontend && npm run build
```
Bei Fehlern (TypeScript, ESLint): Sofort beheben, dann erneut prüfen.

### 3.3 UI Smoke-Test mit Playwright MCP

Starte die Anwendung falls noch nicht laufend:
- Frontend: `cd frontend && npm run dev` (Port 5173)

Führe mit dem Playwright MCP einen Smoke-Test des Happy Flows durch:
1. Navigiere zur relevanten Seite der Story
2. Führe den Hauptflow aus (die beschriebene User Journey)
3. Mache Screenshots an wichtigen Schritten
4. Prüfe visuelle Korrektheit

### 3.4 Akzeptanzkriterien-Check
Gehe jedes Akzeptanzkriterium der Story durch:
- [ ] Kriterium 1: Screenshot / Beobachtung als Nachweis
- [ ] Kriterium 2: ...
- (für jedes AK aus der Story)

Markiere erfüllte AKs mit ✅ und nicht erfüllte mit ❌ (mit Erklärung).

### 3.5 Abschlussbericht
Kurze Zusammenfassung:
- Was wurde implementiert
- Welche AKs sind erfüllt
- Falls etwas offen ist: was und warum
</instructions>

<conventions>
- Story-Dateien: `docs/produkt/backlog/CLVN-XXX-STORY-*.md`
- Frontend-Seiten: PascalCase Deutsch (z.B. `RaumBuchen.tsx`)
- Backend-Klassen: PascalCase Englisch (z.B. `MapBookings`)
- API-Routes: englisch, kebab-case (z.B. `/api/conference-rooms`)
- DTOs: `CreateBookingRequest`, `BookingResponse` etc.
- Conventional Commits für alle Commits
</conventions>

<task>
Entwickle die User Story CLVN-$ARGUMENTS vollständig.

Starte mit Phase 1: Lies die Story-Datei, analysiere den Code und erstelle einen Implementierungsplan zur Review.
</task>
