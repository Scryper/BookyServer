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
- Use primary constructors for simple dependency injection when they remain readable. Throw `ArgumentNullException` when a dependency is null in the constructor.
- Name asynchronous methods with the `Async` suffix and put `CancellationToken` last.
- Use `var` where the right-hand side makes the type clear. Keep nullable annotations accurate.
- Always use statement bodies for methods
- Never map like `houses.Select(house => new HouseDto(house.Id, house.Number))`, create a dedicated mapper and use it `houses.Select(HouseMapper.Map)`
- Always use `this` qualifier when accessing instance fields or properties
- There should be no magic strings. When a string is needed for a case or another, either add the string as a `private const string` at top of the file, or in a project scoped `Constants` static class that contains every string used in the project
  - Group the strings by scope in that file by putting them into a public static class inside the Constants static class
