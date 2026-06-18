# ADR-003: Ressource-Service als Mock-Daten in der SPA

**Status**: Akzeptiert

**Datum**: 2026-06-17

---

## Kontext

Das Calvin-System benötigt Stammdaten zu Standorten, Konferenzräumen und Ausstattungen. Für den produktiven Betrieb wäre ein dedizierter Ressource-Service sinnvoll. Für den Prototypen stellt sich die Frage, wie diese Daten bereitgestellt werden.

Anforderungen:
- Standorte, Räume und Ausstattungen müssen in der SPA anzeigbar sein
- Der Booking Service muss Buchungen einem Raum zuordnen können
- Möglichst geringe Komplexität für schnelles Prototyping

---

## Betrachtete Optionen

### Option 1: Eigener Ressource-Service (Backend)

**Vorteile:**
- Stammdaten zentral verwaltbar und änderbar ohne Deployment
- Produktionsreife Architektur von Beginn an

**Nachteile:**
- Erhöhter Entwicklungsaufwand für den Prototypen
- Zusätzlicher Service erhöht die operative Komplexität

---

### Option 2: Mock-Daten in der SPA (gewählt)

**Vorteile:**
- Kein zusätzlicher Service notwendig
- Sofort einsatzbereit ohne Datenbankmigrationen
- Ausreichend für die Validierung des Buchungsflows im Prototypen

**Nachteile:**
- Stammdaten sind im Deployment fest verdrahtet — Änderungen erfordern ein neues Frontend-Deployment
- Nicht produktionstauglich; muss vor Go-Live abgelöst werden (→ technische Schuld)

---

## Entscheidung

Für den Prototypen werden Standorte, Konferenzräume und Ausstattungen als **statische Mock-Daten in der SPA** hinterlegt. Der Booking Service arbeitet ausschließlich mit den **IDs** aus diesen Mock-Daten und verwaltet keine eigenen Ressourcen-Stammdaten.

---

## Begründung

- Der Prototyp dient der Validierung des Buchungsflows — nicht der Stammdatenverwaltung.
- Der Booking Service bleibt schlank: Er muss nur Buchungen gegen bekannte IDs prüfen.
- Der spätere Austausch gegen einen echten Ressource-Service ist durch die klare ID-Schnittstelle möglich.

---

## Konsequenzen

- Mock-Daten liegen in der SPA (z. B. als TypeScript-Konstanten unter `frontend/src/data/`).
- Der Booking Service akzeptiert Raum-IDs aus den Mock-Daten, ohne sie zu validieren.
- Vor dem produktiven Betrieb muss ein Ressource-Service implementiert werden, der die Mock-Daten ablöst (technische Schuld T-001, siehe [technische-schulden.md](../technische-schulden.md)).
