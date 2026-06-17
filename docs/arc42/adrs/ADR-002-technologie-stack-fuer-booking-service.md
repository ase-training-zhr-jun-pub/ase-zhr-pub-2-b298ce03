# ADR-002: Technologie-Stack für den Booking Service

**Status**: Akzeptiert

**Datum**: 2026-06-17

---

## Kontext

Das Calvin-System besteht aus einer React-SPA und einem separaten Booking Service (siehe ADR-001 in `docs/arc42/adrs/`). Für den Booking Service steht die Technologieentscheidung noch aus.

Anforderungen an den Booking Service:

- REST API (JSON über HTTPS)
- Kompatibilität mit dateibasierter Datenbank (SQLite) für einfaches Deployment
- Schnelle Entwicklung
- Zuverlässige Verhinderung von Doppelbuchungen (QS-2: 99,9%)
- Perspektivisch: Okta-Integration für Single Sign-On — für den Prototypen durch Basic-Auth ersetzt (siehe ADR-004)

---

## Betrachtete Optionen

### Option 1: ASP.NET Core (C# / .NET 8)

**Vorteile:**
- Erstklassige REST API Unterstützung via Minimal APIs oder Web API Controller
- EF Core + SQLite: vollständige ORM-Unterstützung inkl. Migrationen
- Okta-Integration über `Microsoft.AspNetCore.Authentication.OpenIdConnect` standardisiert
- Built-in Transaktionsunterstützung für Doppelbuchungs-Prävention
- Schnelle Entwicklung durch .NET CLI Scaffolding, eingebaute DI, Swagger/OpenAPI
- Bekannte Technologie im Team → kein Einarbeitungsaufwand

**Nachteile:**
- Etwas mehr Boilerplate als Python/Node für einfache Endpunkte

---

### Option 2: Spring Boot (Java / Kotlin)

**Vorteile:**
- Ausgereiftes Ökosystem, Okta Spring Boot Starter vorhanden
- H2 (Datei-Modus) und SQLite gut unterstützt

**Nachteile:**
- JVM: höherer Ressourcenverbrauch, längere Startzeiten
- Längere Einarbeitungszeit wenn .NET bekannt ist
- Komplexeres Build-System (Maven/Gradle) im Vergleich zu .NET CLI

---

### Option 3: Node.js / TypeScript (Fastify + Prisma)

**Vorteile:**
- Konsistente Sprache mit dem Frontend (TypeScript)
- Prisma ORM unterstützt SQLite nativ
- Sehr schneller Einstieg für kleine Services

**Nachteile:**
- Single-threaded Event Loop: bei gleichzeitigen Buchungsanfragen (QS-2) fehlt echte Parallelität auf Thread-Ebene
- Okta-Integration weniger standardisiert (Drittbibliotheken)
- Schwächeres statisches Typsystem als C# für komplexe Domänenlogik (Buchungsregeln, Zeitüberschneidungen)

---

## Entscheidung

Wir verwenden **ASP.NET Core (.NET 8, C#)** als Technologie-Stack für den Booking Service.

---

## Begründung

- **Anforderungserfüllung**: Alle technischen Anforderungen werden direkt und ohne Workarounds erfüllt — REST API, SQLite via EF Core, Okta via Standard-Middleware.
- **Zuverlässigkeit (QS-2)**: EF Core unterstützt Datenbankisolation und optimistische Nebenläufigkeitskontrolle, um Doppelbuchungen transaktionssicher zu verhindern.
- **Okta**: `Microsoft.AspNetCore.Authentication.OpenIdConnect` ist der standardisierte Weg für OIDC in .NET — minimaler Konfigurationsaufwand bei späterer Integration.
- **Entwicklungsgeschwindigkeit**: Da das Team .NET kennt, entfällt Einarbeitungsaufwand. .NET CLI, Scaffolding und eingebaute OpenAPI-Generierung beschleunigen die Entwicklung.
- **Migrationspfad**: EF Core ermöglicht den späteren Wechsel von SQLite zu PostgreSQL ohne Änderungen an der Geschäftslogik.

---

## Konsequenzen

- Das Backend-Projekt liegt unter `backend/` und wird als .NET 8 Web API Projekt angelegt.
- Datenbank: SQLite über EF Core mit Code-First Migrationen.
- OpenAPI-Spezifikation wird automatisch aus den Controllern/Minimal-API-Definitionen generiert.
- **Prototyp**: Authentifizierung über Basic-Auth ohne Passwörter (ADR-004); Okta-Integration ist für den produktiven Betrieb vorgesehen.
- Bei Okta-Integration: `Microsoft.AspNetCore.Authentication.OpenIdConnect` + Okta-spezifische Konfiguration in `appsettings.json`.
