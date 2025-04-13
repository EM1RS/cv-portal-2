# CV-Portal API

Dette er et ASP.NET Core Web API-prosjekt som tilbyr funksjonalitet for å opprette, hente, oppdatere og slette digitale CV-er. 
Prosjektet er laget som en del av skoleprosjekt ved Gokstad Akademiet.

## Teknologier brukt

- ASP.NET Core 8
- Entity Framework Core
- Identity med JWT-autentisering
- MySQL (kjørt i Docker container)
- Docker & Docker Compose
- Swagger (OpenAPI)

## Prosjektstruktur

- `Controllers/` – API-endepunkter
- `Services/` – Forretningslogikk
- `Repositories/` – Databaseaksess
- `Models/` – Datamodeller
- `DTO/` – Data Transfer Objects
- `Data/` – EF DbContext og seeding
- `Migrations/` – EF migrasjoner

## GIT

git clone https://github.com/EM1RS/cv-portal-2.git

Pass på at du er i riktig mappe (CvApi2)  

## Kjøre API-et
1.  dotnet run / dotnet watch
    API-et vil da være tilgjengelig på:
    http://localhost:5005/api/users - for å hente users
    http://localhost:5005/api/cvs   - for å hente cv-er / feks med postman (etter autorisering)


2.  Gå til http://localhost:5005/swagger i nettleseren for å utforske og teste API-endepunktene.

3.  api/login
    POST denne brukeren
    {
    "email": "eso@trimit.com",
    "password": "AdminPass2025!"
    }
    Du får tilbake e JWT-token.

4. Autoriser deg i Swagger.
    Bearer + tokenet du får tilbake
    trykk Authorize og Close

5. Nå kan du: 
    Opprette nye brukere: /api/users
    Opprette CV-er for brukere: /api/cvs/for-user/{userId}
    Hente alle brukere eller CV-er
    Hente alle brukere eller Brikere
    Oppdatere og slette CV-er
    Oppdatere og slette Brukere
    osv osv. 
