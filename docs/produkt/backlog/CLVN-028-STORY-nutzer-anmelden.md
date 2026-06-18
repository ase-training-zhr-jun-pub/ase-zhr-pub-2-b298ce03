---
Ticket-ID: CLVN-028
Type: Story
Status: TODO
---
# Nutzer anmelden

## User Story

Als INNOQ-Mitarbeiter möchte ich mich im Calvin-Prototypen mit meinem Namen anmelden, damit meine Buchungen meiner Identität zugeordnet werden.

## Beschreibung

Beim Aufruf von Calvin ohne aktive Sitzung wird der Mitarbeiter zu einer Login-Seite weitergeleitet. Dort wählt er seinen Namen aus einer Liste bekannter Testnutzer aus. Ein Passwort ist im Prototypen nicht erforderlich. Nach der Auswahl ist der Mitarbeiter angemeldet und kann Räume suchen und buchen.

Diese vereinfachte Authentifizierung ermöglicht das schnelle Testen des Systems mit verschiedenen Nutzeridentitäten, ohne Abhängigkeit zu einem externen Identity-Provider (Okta).

> **Hinweis:** Diese Story gilt nur für den Prototypen. Im produktiven System wird die Anmeldung durch Okta/SSO ersetzt (technische Schuld T-002).

## Akzeptanzkriterien

- [ ] Eine Login-Seite wird angezeigt, wenn kein Nutzer angemeldet ist
- [ ] Die Login-Seite zeigt eine Auswahlliste mit Testnutzern
- [ ] Nach Auswahl eines Nutzers ist dieser angemeldet und wird zur Startseite weitergeleitet
- [ ] Der angemeldete Nutzername wird in der Oberfläche sichtbar angezeigt
- [ ] Buchungen werden mit der Identität des angemeldeten Nutzers gespeichert
- [ ] Es gibt eine Möglichkeit, sich abzumelden

## Betroffene Persona

[INNOQ-Mitarbeiter](/docs/produkt/personas/innoq-mitarbeiter.md)
