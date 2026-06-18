# ADR-004: Basic-Auth ohne Passwörter für den Prototypen

**Status**: Akzeptiert

**Datum**: 2026-06-17

---

## Kontext

Das Calvin-System benötigt einen Mechanismus zur Nutzeridentifikation, damit Buchungen einem Mitarbeiter zugeordnet werden können. Für den produktiven Betrieb ist eine Okta-Integration (OIDC/SSO) vorgesehen (siehe ADR-002). Für den Prototypen stellt sich die Frage, wie Authentifizierung ohne externe Abhängigkeiten umgesetzt werden kann.

Anforderungen an die Prototypen-Lösung:
- Verschiedene Nutzeridentitäten testbar
- Keine Abhängigkeit zu Drittsystemen (Okta, LDAP)
- Minimaler Implementierungsaufwand

---

## Betrachtete Optionen

### Option 1: Okta-Integration (OIDC)

**Vorteile:**
- Produktionsreife Authentifizierung
- Direkt einsatzbereit für echte INNOQ-Accounts

**Nachteile:**
- Abhängigkeit zu Okta-Tenant-Konfiguration
- Erhöhter Aufwand (Tenant-Setup, Callback-URLs, Secret-Management)
- Blockiert Prototyp-Entwicklung bei Okta-Problemen

---

### Option 2: Basic-Auth ohne Passwörter (gewählt)

Mitarbeiter wählen beim Login ihren Namen aus einer Liste bekannter Testnutzer. Ein Passwort wird nicht geprüft.

**Vorteile:**
- Kein Drittsystem notwendig
- Schneller Nutzerwechsel für Tests
- Minimale Implementierung

**Nachteile:**
- Keine echte Authentifizierung — nicht sicherheitsrelevant einsetzbar
- Muss vor Go-Live durch Okta-Integration ersetzt werden (→ technische Schuld)

---

## Entscheidung

Für den Prototypen wird **Basic-Auth ohne Passwortprüfung** eingesetzt. Nutzer wählen ihre Identität aus einer fest konfigurierten Liste von Testnutzern aus. Der Booking Service vertraut der übermittelten Nutzeridentität ohne kryptographische Verifikation.

---

## Begründung

- Der Prototyp dient der Validierung der Buchungsfunktionalität mit verschiedenen Nutzern — nicht der Absicherung gegen unautorisierte Zugriffe.
- Die Entkopplung von Okta ermöglicht parallele Entwicklung ohne externe Abhängigkeiten.
- Die Okta-Integration ist für den Produktionsgang bereits architektonisch vorbereitet (ASP.NET Core OIDC-Middleware, siehe ADR-002).

---

## Konsequenzen

- Login-Seite zeigt eine Auswahlliste bekannter Testnutzer; Passwortfeld entfällt.
- Der Booking Service nimmt die Nutzer-ID aus dem Request entgegen, ohne Signaturprüfung.
- Vor dem produktiven Betrieb muss die Okta-Integration implementiert werden (technische Schuld T-002, siehe [technische-schulden.md](../technische-schulden.md)).
- Das System ist im Prototypen-Zustand **nicht für den öffentlichen Betrieb geeignet**.
