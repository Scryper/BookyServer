---
name: ef-core-schema-change
description: Change BookyServer's EF Core model and create or review its SQL Server migration.
---

Use this skill when changing persisted entities, their EF Core configuration, or a migration.

1. Update the domain model and its `IEntityTypeConfiguration` together. Update `BookyServerDbContext`, relationships, repositories, and seed data when the change requires them.
2. Generate a descriptive migration from the repository root after the model is complete:

```powershell
dotnet ef migrations add <MigrationName> --project BookyServer.Infrastructure --startup-project BookyServer.Api --output-dir Persistence/Migrations
```

3. Review the generated `Up` and `Down` operations for data loss, locks, defaults, and rollout compatibility. Do not alter an already-applied migration.
4. Apply migrations only to the intended environment. Treat production rollout as a separate, reviewed deployment step.
