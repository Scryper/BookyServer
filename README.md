# BookyServer

API REST ASP.NET Core pour l'application Booky, une plateforme de rencontres entre lecteurs.

## Responsabilités prévues

- Comptes et authentification.
- Profils, intérêts, livres lus et médias.
- Suggestions et gestion de groupes de lecture (8 membres maximum).
- Conversations, messages et votes de rencontre.
- Lieux et marqueurs de carte.

## Architecture

Solution .NET découpée en cinq projets :

- `BookyServer.Api` : endpoints HTTP et composition de l'application.
- `BookyServer.Interfaces` : contrats d'accès aux données et services.
- `BookyServer.Application` : services et règles d'application.
- `BookyServer.Infrastructure` : EF Core, SQL Server, migrations et configurations.
- `BookyServer.Domain` : entités métier persistées.

Les DTO et mappers manuels gardent le modèle de domaine à l'écart des contrats HTTP.

## Démarrer

Prérequis : SDK .NET 10 et Docker.

Les paramètres encore à fournir sont signalés par `TODO`. Ne pas committer de secrets.

```bash
dotnet restore
dotnet build
dotnet test
docker compose up --build
```

L'API publie son contrat OpenAPI en environnement de développement et expose des sondes de santé. Les migrations EF Core sont gérées avec `dotnet ef` depuis le projet API en ciblant le projet Infrastructure.

Voir `src/` pour les projets et `docker-compose.yml` pour l'environnement local SQL Server.
