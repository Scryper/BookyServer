# BookyServer

## Skills

Use `$code-style` for every task.

Use `$api-change` when adding or changing an HTTP endpoint, `$ef-core-schema-change` for a persisted-model or migration change, and `$api-security-change` for authentication, authorization, CORS, cookies, configuration, or security middleware.

## Architecture

- `BookyServer.Domain` contains persistent domain models and domain rules.
- `BookyServer.Interfaces` owns DTO contracts and service or repository abstractions.
- `BookyServer.Application` validates and orchestrates use cases through interfaces; keep HTTP and EF Core details out of it.
- `BookyServer.Infrastructure` owns EF Core, SQL Server mappings, repositories, and ASP.NET Identity storage.
- `BookyServer.Api` owns routing, authorization, middleware, and dependency composition. Controllers call application services rather than repositories.

Keep new feature files together in their subject folder, such as `Profiles`, `Books`, `ReaderGroups`, `Conversations`, or `MapMarkers`.

## Project conventions

- The target is .NET 10 with nullable reference types and warnings treated as errors.
- Keep public HTTP contracts in `BookyServer.Interfaces.Contracts` and expose versioned routes under `api/v1`.
- Pass the request `CancellationToken` through asynchronous service and repository calls.
- Keep credentials out of source control. Runtime configuration comes from environment variables or local configuration.

## Verification

Build the solution after source changes with:

```powershell
dotnet build BookyServer.sln --no-restore
```

There is no test project yet. For container changes, also run `docker compose config --quiet` and build the production image when practical.
