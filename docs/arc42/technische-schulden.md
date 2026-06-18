# Technische Schulden

Bewusst eingegangene Kompromisse für den Calvin-Prototypen, die vor dem produktiven Betrieb abgelöst werden müssen.

| ID | Beschreibung | Entstanden durch | Auswirkung | Priorität vor Go-Live |
|----|--------------|------------------|------------|----------------------|
| T-001 | **Mock-Daten in der SPA** — Standorte, Konferenzräume und Ausstattungen sind als statische Konstanten im Frontend hinterlegt. Änderungen erfordern ein Frontend-Deployment. | [ADR-003](adrs/ADR-003-ressource-service-als-mock-in-spa.md) | Keine echte Stammdatenverwaltung; Raum-/Standortdaten nicht admin-seitig pflegbar | Hoch |
| T-002 | **Basic-Auth ohne Passwörter** — Nutzer wählen ihre Identität ohne Passwortprüfung. Das System ist nicht gegen unautorisierte Zugriffe abgesichert. | [ADR-004](adrs/ADR-004-basic-auth-ohne-passwort-fuer-prototyp.md) | Kein Schutz von Buchungsdaten; nicht für echte Nutzerdaten geeignet | Kritisch |
| T-003 | **SQLite als Datenbank** — Für den Prototypen wird SQLite eingesetzt. SQLite skaliert nicht für mehrere gleichzeitige Schreibzugriffe in einem verteilten Deployment. | [ADR-002](adrs/ADR-002-technologie-stack-fuer-booking-service.md) | Begrenzte Nebenläufigkeit; kein Replikat-Betrieb möglich | Mittel |

## Abnahmebedingungen vor Go-Live

- [ ] T-001: Ressource-Service implementiert und Mock-Daten aus SPA entfernt
- [ ] T-002: Okta-Integration aktiviert; Basic-Auth deaktiviert
- [ ] T-003: PostgreSQL als Datenbankbackend konfiguriert und migriert
