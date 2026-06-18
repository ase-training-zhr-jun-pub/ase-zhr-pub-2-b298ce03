---
Ticket-ID: CLVN-029
Type: Story
Status: TODO
---
# Nutzeridentität wechseln

## User Story

Als Tester möchte ich im Prototypen schnell zwischen verschiedenen Nutzeridentitäten wechseln können, damit ich das System aus verschiedenen Perspektiven testen kann.

## Beschreibung

Im Prototypen wird kein echtes SSO verwendet. Damit Tester und Entwickler dennoch verschiedene Nutzerperspektiven einnehmen können — z. B. um zu prüfen, ob Buchungen der richtigen Person zugeordnet werden — soll ein einfacher Nutzerwechsel möglich sein.

Der Nutzer meldet sich ab und wählt auf der Login-Seite eine andere Testidentität aus. Die vorherige Sitzung wird dabei vollständig beendet.

> **Hinweis:** Diese Funktionalität ist auf den Prototypen beschränkt. Im produktiven System erfolgt der Identitätswechsel über Okta.

## Akzeptanzkriterien

- [ ] Ein "Abmelden"-Button ist in der Navigation sichtbar
- [ ] Nach dem Abmelden wird der Nutzer zur Login-Seite weitergeleitet
- [ ] Auf der Login-Seite kann eine andere Testidentität ausgewählt werden
- [ ] Nach dem Wechsel sind keine Daten der vorherigen Sitzung im UI sichtbar
- [ ] Buchungen der vorherigen Identität bleiben im System erhalten und sind nach erneutem Anmelden mit derselben Identität wieder abrufbar

## Betroffene Persona

[INNOQ-Mitarbeiter](/docs/produkt/personas/innoq-mitarbeiter.md)
