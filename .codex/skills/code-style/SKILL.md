---
name: code-style
description: Apply BookyServer's C# style and source layout to every repository task.
---

Use this skill for every task. Follow explicit user instructions when they conflict with this guidance.

- Use file-scoped namespaces, four-space indentation, and Allman braces.
- Declare one type per file. Name the file after that type, including DTO records and enums.
- Group files by feature subject. Do not place unrelated types directly in a catchall folder such as `Entities` or `Contracts`.
- Prefer `sealed` classes unless inheritance is intentional. Keep mappers `internal static`.
- Keep contracts as `sealed record` types and put each contract in its subject folder.
- Use primary constructors for simple dependency injection when they remain readable. Name asynchronous methods with the `Async` suffix and put `CancellationToken` last.
- Use `var` where the right-hand side makes the type clear. Keep nullable annotations accurate.
