# 3. Kontextabgrenzung

## Überblick

Das Calvin-System ist INNOQs internes Raum- und Arbeitsplatzbuchungssystem. Das System operiert in einem minimalen Systemkontext.

## Fachlicher Kontext

```plantuml
@startuml
actor "Consultant" as Consultant
actor "Geschäftsleitung" as GL

rectangle "Calvin" as Calvin

Consultant --> Calvin : Bucht Räume &\n Arbeitsplätze
GL --> Calvin : Sieht Reports
@enduml
```

| Nachbarsystem / Akteur | Beziehung |
|------------------------|-----------|
| **INNOQ Mitarbeiter (Consultant)** | Sucht und bucht Räume & Arbeitsplätze |
| **Geschäftsleitung** | Ruft Auslastungsreports ab |

## Technischer Kontext

<!-- Technische Schnittstellen und Protokolle zu externen Systemen -->

| Schnittstelle | Protokoll / Format | Richtung |
|---------------|--------------------|----------|
| | | |
