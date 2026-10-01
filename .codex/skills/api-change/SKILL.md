---
name: api-change
description: Add or change a BookyServer HTTP endpoint across its controller, contract, service, and persistence layers.
---

Use this skill when a request adds or changes an endpoint or its request or response contract.

1. Keep the route under `api/v1` and decide whether it is explicitly anonymous, authenticated, or policy-protected.
2. Put each request, response, and result contract in `BookyServer.Interfaces.Contracts/<Subject>`.
3. Keep controllers thin: obtain the authenticated user from Identity or claims, call an application service, and return the documented HTTP status codes and `ProblemDetails` for validation errors.
4. Carry the behavior through the service interface, application service, and repository abstraction or implementation only where persistence is needed. Keep EF Core out of controllers and application services.
5. Pass the request cancellation token through every asynchronous call and update `ProducesResponseType` attributes to match the outcome.
