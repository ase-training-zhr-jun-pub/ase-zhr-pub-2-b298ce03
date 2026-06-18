# Qualitätsanforderungen

Dieses Dokument beschreibt die wesentlichen Qualitätsanforderungen des Calvin-Systems
in Form von **Qualitätsszenarien** gemäß [arc42 Abschnitt 10](https://docs.arc42.org/section-10/).

Jedes Szenario ist in die sechs Bestandteile eines arc42-Qualitätsszenarios gegliedert:

| Bestandteil | Bedeutung |
|-------------|-----------|
| **Environment** | Betriebssituation / Rahmenbedingungen, unter denen das Szenario eintritt |
| **Source** | Auslöser des Ereignisses (Nutzer, System, Angreifer, Infrastruktur) |
| **Event** | Das auslösende Ereignis (Stimulus) |
| **Artifact** | Der betroffene Teil des Systems |
| **Response** | Die erwartete Reaktion des Systems |
| **Measure** | Messbares Kriterium, an dem die Reaktion bewertet wird |

## Priorisierung

1. **Hosting-Kosten** (QS-1)
2. **Performance** (QS-2)
3. **Security** (QS-3)
4. **Benutzbarkeit** (QS-4)
5. **Zuverlässigkeit** (QS-5) — oberstes Qualitätsziel von Calvin, daher ergänzend aufgenommen

## Qualitätsszenarien

| ID | Qualitätsmerkmal | Environment | Source | Event | Artifact | Response | Measure |
|----|------------------|-------------|--------|-------|----------|----------|---------|
| **QS-1** | Hosting-Kosten | Normalbetrieb außerhalb der Kernarbeitszeiten (nachts, Wochenende), nahezu keine Last | Cloud-Infrastruktur | Über mehrere Stunden gehen keine Buchungsanfragen ein | Hosting-Infrastruktur (Compute / Container) | Das System skaliert ungenutzte Compute-Ressourcen automatisch herunter (Scale-to-Zero) | Monatliche Hosting-Kosten bleiben unter dem Budget von **X €/Monat**; Compute ist an ≥ 8 h/Tag auf null skaliert |
| **QS-2** | Performance | Normalbetrieb während der Kernarbeitszeiten, bis zu 150 gleichzeitige Nutzer | INNOQ-Mitarbeiter (Consultant) | Mitarbeiter sucht auf der Calvin-Website verfügbare Räume an einem Standort für einen Zeitraum | Calvin-Weboberfläche / Such-API | Die Suchergebnisliste wird angezeigt und ist interaktiv (First Contentful Paint) | Ergebnisse sichtbar und interaktiv in < 500 ms für 95 % der Anfragen |
| **QS-3** | Security | Normalbetrieb, öffentlich erreichbarer Endpunkt | Nicht authentifizierter / unautorisierter Akteur | Zugriffsversuch auf Buchungsdaten ohne gültiges Token (abgelaufen, ungültig oder fehlend) | API / Authentifizierungsschicht | Das System weist den Zugriff ab und protokolliert den Versuch | 100 % der Anfragen ohne gültiges Token werden mit HTTP 401 abgelehnt; jeder Vorfall steht im Audit-Log |
| **QS-4** | Benutzbarkeit | Erstnutzung ohne vorherige Schulung | Neuer INNOQ-Mitarbeiter | Mitarbeiter bucht zum ersten Mal einen Arbeitsplatz für den nächsten Tag | Calvin-Weboberfläche / Buchungsflow | Mitarbeiter findet intuitiv durch die Oberfläche und schließt die Buchung selbstständig ab | Buchung in ≤ 5 Minuten und ≤ 20 Klicks; 90 % der neuen Mitarbeiter schaffen es ohne Hilfe |
| **QS-5** | Zuverlässigkeit | Normalbetrieb, konkurrierende Buchungsversuche | Zwei INNOQ-Mitarbeiter | Beide buchen denselben Raum für denselben Zeitraum innerhalb derselben Sekunde | Booking-Service / Persistenz | Die erste vollständige Anfrage wird erfolgreich verarbeitet, die zweite mit verständlicher Fehlermeldung abgelehnt | Doppelbuchungen werden in 99,9 % der Fälle serverseitig verhindert |

> **Offen:** Das Budget in QS-1 (`X €/Monat`) ist ein Platzhalter — sobald eine konkrete
> monatliche Obergrenze feststeht, hier eintragen, damit das Measure vollständig testbar ist.
