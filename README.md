# BookyServer

API REST ASP.NET Core pour Booky, l'application de rencontre entre lecteurs.

## Périmètre

- Authentification et comptes.
- Profils, intérêts et livres lus.
- Groupes de lecture de 8 personnes maximum.
- Messages et échanges dans les groupes.
- Lieux et marqueurs de carte.

## Architecture

- `BookyServer.Api` : point d'entrée HTTP, contrôleurs et composition de l'application.
- `BookyServer.Interfaces` : interfaces de services/dépôts et contrats DTO.
- `BookyServer.Application` : orchestration métier et mappers manuels.
- `BookyServer.Infrastructure` : EF Core, SQL Server, configurations et migrations.
- `BookyServer.Domain` : entités persistées et règles du domaine.

Le stockage SQL Server est géré avec EF Core. L'authentification HTTP utilise les endpoints ASP.NET Core Identity API. OpenAPI est disponible en développement. Le déploiement cible des conteneurs Docker sur Infomaniak.

## Démarrage

Prérequis : SDK .NET 10 et Docker Compose.

1. Remplacer les valeurs `TODO` et ne pas committer de secrets.
2. Définir un mot de passe local SQL Server :

   ```bash
   export MSSQL_SA_PASSWORD='TODO_Replace_With_A_Strong_Local_Password_2026!'
   docker compose up -d sqlserver
   ```

3. Créer la base locale (remplacer le secret par celui de l'étape précédente) :

   ```bash
   docker compose exec sqlserver /opt/mssql-tools18/bin/sqlcmd \
     -C -S localhost -U sa -P "$MSSQL_SA_PASSWORD" \
     -Q "IF DB_ID(N'BookyServer') IS NULL CREATE DATABASE [BookyServer]"
   ```

4. Restaurer, compiler et appliquer la première migration :

   ```bash
   dotnet restore
   dotnet build BookyServer.sln --no-restore
   dotnet tool install --global dotnet-ef --version 10.0.11
   dotnet ef migrations add InitialCreate \
     --project src/BookyServer.Infrastructure \
     --startup-project src/BookyServer.Api \
     --output-dir Persistence/Migrations
   ```

   Pour la mise à jour locale, transmettre aussi la chaîne SQL Server au processus `dotnet ef` :

   ```bash
   ConnectionStrings__BookyDatabase="Server=localhost,1433;Database=BookyServer;User Id=sa;Password=$MSSQL_SA_PASSWORD;Encrypt=True;TrustServerCertificate=True;" \
     dotnet ef database update \
       --project src/BookyServer.Infrastructure \
       --startup-project src/BookyServer.Api
   ```

5. Démarrer l'API et SQL Server :

   ```bash
   docker compose up --build
   ```

L'API écoute sur `http://localhost:8081`, OpenAPI en développement sur `/openapi/v1.json`, et les sondes sur `/health/live` et `/health/ready`.

## Configuration à compléter

- `ConnectionStrings:BookyDatabase` : chaîne SQL Server locale ou Infomaniak.
- `Authentication:RequireConfirmedEmail` et l'expéditeur/transport mail : à définir avant toute inscription publique.
- `Cors:AllowedOrigins` : origines exactes du site Next.js.
- `MediaStorage` : fournisseur, bucket, endpoint, identifiants et politique d'accès.
- SMTP, journaux, sauvegardes, limites d'upload, clé de protection des cookies et politique d'administration.
- Secrets Docker/CI et règles de pare-feu : gestion manuelle dans l'environnement de déploiement Infomaniak.
- Cette première initialisation n'implémente pas encore l'envoi des emails de confirmation. `RequireConfirmedEmail` reste désactivé en développement ; intégrer et tester un expéditeur SMTP avant toute inscription publique.
- Créer/attribuer le rôle `Admin` hors API avant d'autoriser la création de marqueurs ; aucun compte n'est administrateur par défaut.

Les placeholders `TODO` ne sont pas des secrets fonctionnels. Configurer les variables d'environnement hors du dépôt pour tout déploiement réel.

## Endpoints initiaux

- `POST /api/v1/auth/register`, `POST /api/v1/auth/login`, `POST /api/v1/auth/refresh`, `POST /api/v1/auth/logout` : endpoints Identity.
- `GET /api/v1/profiles/me`, `PUT /api/v1/profiles/me`, `GET /api/v1/profiles/{id}`.
- `GET /api/v1/books?query=...`, `PUT /api/v1/profiles/me/books/{bookId}`.
- `GET /api/v1/groups`, `GET /api/v1/groups/{id}/members`, `POST` et `DELETE /api/v1/groups/{id}/join`.
- `GET /api/v1/groups/{id}/messages`, `POST /api/v1/groups/{id}/messages`.
- `GET /api/v1/map-markers`; gestion CRUD réservée à un futur rôle administrateur.

Les requêtes portant sur les profils et conversations exigent une authentification. La capacité du groupe est vérifiée en transaction sérialisable. Les migrations de production doivent être générées, relues et déployées comme une étape contrôlée, avant le démarrage de la nouvelle version API.
