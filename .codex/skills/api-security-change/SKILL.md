---
name: api-security-change
description: Safely change BookyServer authentication, authorization, CORS, cookies, configuration, or security middleware.
---

Use this skill when changing Identity, authorization, CORS, cookies, configuration, security headers, or request-filtering middleware.

- Keep protected routes protected by default; add `AllowAnonymous` only for intentionally public behavior. Enforce privileged operations with named policies such as `AdminOnly`.
- Derive the acting user from the authenticated principal or `UserManager`, never from request input.
- Keep CORS origins explicit from `Cors:AllowedOrigins`; preserve credentialed CORS requirements and do not widen origins for convenience.
- Preserve secure, HTTP-only, strict-site cookies unless a change is explicitly required and its cross-site behavior is addressed.
- Keep secrets, connection strings, and credentials in environment variables or local configuration. Do not log them or commit them.
