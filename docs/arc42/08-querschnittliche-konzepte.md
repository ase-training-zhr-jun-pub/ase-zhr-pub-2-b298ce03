# 8. Querschnittliche Konzepte

<!-- Übergreifende, fachliche und technische Konzepte, die mehrere Bausteine betreffen. -->

## Authentifizierung & Autorisierung

**Prototyp:** Basic-Auth ohne Passwortprüfung (siehe [ADR-004](adrs/ADR-004-basic-auth-ohne-passwort-fuer-prototyp.md)). Nutzer wählen ihre Identität aus einer fest konfigurierten Liste von Testnutzern. Kein externer Identity-Provider.

**Produktion (geplant):** Okta-Integration via OIDC (`Microsoft.AspNetCore.Authentication.OpenIdConnect`). Die ASP.NET-Core-Middleware ist architektonisch bereits vorbereitet (siehe ADR-002).

| Aspekt | Prototyp | Produktion |
|--------|----------|------------|
| Mechanismus | Basic-Auth, kein Passwort | Okta OIDC / SSO |
| Nutzerquelle | Statische Testnutzerliste | INNOQ Okta-Tenant |
| Token-Verifikation | Keine | JWT-Signaturprüfung |

## Fehlerbehandlung

<!-- Einheitliche Strategie für Fehlermeldungen (API-Fehlercodes, UI-Fehlertexte) -->

## Logging & Monitoring

<!-- Was wird geloggt? Wie werden Fehler und Audit-Ereignisse protokolliert? -->

## Datenpersistenz

<!-- Datenbankstrategie, Transaktionen, Konfliktbehandlung bei gleichzeitigen Buchungen -->

## API-Design

<!-- Konventionen für die REST-API (Versionierung, Fehlerformat, Paginierung) -->
