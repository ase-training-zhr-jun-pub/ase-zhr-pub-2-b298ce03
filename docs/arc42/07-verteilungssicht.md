# 7. Verteilungssicht

<!-- Beschreibung der technischen Infrastruktur und der Zuordnung von Bausteinen zu Infrastrukturelementen (Deployment-Diagramm). -->

## Infrastruktur

<!-- Auf welcher Plattform / in welcher Cloud wird Calvin betrieben? -->

## Deployment-Diagramm

```plantuml
@startuml
!theme plain

node "Cloud" {
    node "Frontend Hosting" {
        artifact "SPA"
    }
    node "Backend Hosting" {
        artifact "Booking Service"
    }
    database "Datenbank"
}

actor "Browser" as browser
browser --> "SPA"
"SPA" --> "Booking Service" : HTTPS / REST
"Booking Service" --> "Datenbank"
@enduml
```

## Qualitätsmerkmale der Infrastruktur

| Merkmal | Maßnahme |
|---------|----------|
| Scale-to-Zero (Hosting-Kosten) | |
| Verfügbarkeit 98 % | |
