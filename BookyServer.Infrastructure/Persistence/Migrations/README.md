# Migrations EF Core

Générer la migration initiale après avoir configuré le SDK .NET 10 et la connexion de conception :

```bash
dotnet ef migrations add InitialCreate \
  --project BookyServer.Infrastructure \
  --startup-project BookyServer.Api \
  --output-dir Persistence/Migrations
```

Le code de migration et `BookyServerDbContextModelSnapshot` générés doivent être committés avec le changement de modèle. Ne pas appeler `Database.Migrate()` automatiquement au démarrage en production ; appliquer les migrations comme une étape de déploiement contrôlée.
