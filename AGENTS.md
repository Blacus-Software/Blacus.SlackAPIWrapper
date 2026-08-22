# AGENTS.md

> Developer and AI Agent guide for working on `Blacus.SlackAPIWrapper`.

---

## 1. Project Overview

- **Name:** `Blacus.SlackAPIWrapper`
- **Target Framework:** `net10.0` (C# 14)
- **Language / Project settings:** `ImplicitUsings` is **disabled** (always add explicit `using` statements). `Nullable` is **enabled**, but CS86xx warnings are globally suppressed via `<NoWarn>`.
- **Purpose:** A typed, attribute-driven .NET 10 wrapper around the Slack Web API, Slack Socket Mode (WebSocket), and ASP.NET Core request/webhook verification.
- **Projects:**
  - `src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/` — Core library (HTTP client, methods, models, converters, rate limiter).
  - `src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper.AspNetCore/` — ASP.NET Core integration extensions.
- **Packaging:** `GeneratePackageOnBuild` is `True` on the core project; `dotnet build` will produce `.nupkg` artifacts.

---

## 2. Essential Commands

Build the projects individually (recommended due to `.sln` referencing non-tracked test projects):
```bash
dotnet build src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/SlackAPIWrapper.csproj
dotnet build src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper.AspNetCore/SlackAPIWrapper.AspNetCore.csproj
```

Clean build outputs:
```bash
dotnet clean src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/SlackAPIWrapper.csproj
dotnet clean src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper.AspNetCore/SlackAPIWrapper.AspNetCore.csproj
```

> **Note:** Automated test suites (`*.Tests`) do not exist in this repository yet. Always run `dotnet build` on both projects to verify compilation and avoid introducing build breaks.

---

## 3. Architecture & Key Patterns

### 3.1. Method Declaration Pattern (`[SlackMethod]`)
Every Slack Web API endpoint is implemented as a standalone class inheriting from `SlackModel` (or `SlackMethod`) and decorated with `[SlackMethod]`:

```csharp
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Methods
{
    [SlackMethod("users.info", "POST", SlackMethodContentType.ApplicationJson)]
    public class UsersInfo : SlackModel
    {
        [JsonPropertyName("user")]
        public User User { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {
            [JsonPropertyName("user")]
            public string UserId { get; set; }

            [JsonPropertyName("include_locale")]
            public bool? IncludeLocale { get; set; }
        }
    }
}
```

- **Top-level properties** define fields expected in Slack's JSON response.
- **Nested `OptionalArguments` class** (subclassing `SlackOptionalArguments`) defines the request payload/parameters.

### 3.2. Models & Dynamic Data
- All Slack models implement `ISlackModel` or inherit from `SlackModel`.
- Models capture unknown properties with `[JsonExtensionData]` (`Dictionary<string, JsonElement>`) and preserve `SourceJson`.
- Always use `System.Text.Json.Serialization.JsonPropertyNameAttribute` to map Slack's snake_case property names to C# PascalCase properties.

### 3.3. Serialization & Converters
- Project uses `System.Text.Json` exclusively (do **not** introduce `Newtonsoft.Json`).
- Reusable converters live in `Blacus.SlackAPIWrapper.JsonConverters`:
  - `BoolConverter` (handles Slack string `"true"`/`"false"` and integers `0`/`1` to bool)
  - `DateTimeOffsetAsIntConverter`
  - `DateOnlyConverter`, `TimeOnlyConverter`
  - `JsonConverterSettings` provides global serializer options.

### 3.4. Rate Limiting
- `Limiter.cs` enforces Slack's tier-based request limits (Tier 1 = 1 req/min, Tier 2 = 20 req/min, Tier 3 = 50 req/min, Tier 4 = 100 req/min).
- Endpoints can be tagged with `[SlackLimiterTier(SlackRateLimitTier.TierX)]`.

### 3.5. DI & Service Registration
- The DI extension method is `services.AddSlackDependency(slackCredential)` (note: the method name in code is `AddSlackDependency`, not `AddSlackAPIWrapper`).

---

## 4. Coding Conventions & AI Workflow

- **C# Style:** Standard .NET PascalCase for public types/methods/properties, `_camelCase` for private instance fields, `camelCase` for parameters/locals.
- **Imports:** Since `<ImplicitUsings>` is `disable`, include all required namespaces explicitly (`using System;`, `using System.Threading.Tasks;`, etc.).
- **Comments:** Keep comments minimal; only document non-obvious design choices, workarounds, or Slack API quirks.
- **Dependencies:** Do not add third-party packages unless absolutely necessary. Rely on standard Microsoft/System libraries (`Microsoft.Extensions.*`, `System.Text.Json`).
- **Nullability:** Handle null safely; avoid adding suppressions like `!` unless the Slack API contract guarantees the value will be present on successful responses.
- **AI Changelog Maintenance (Mandatory):** Whenever an AI agent makes code or configuration changes in this repository, it **must** update and append the changes to `Documentation/AI_CHANGELOG.md` under the appropriate section in reverse chronological order.

---

## 5. Security Guardrails (High Priority)

1. **Never commit secrets or tokens:** Slack bot tokens (`xoxb-...`), app-level tokens (`xapp-...`), client secrets, and signing secrets must never be hardcoded or written to tracked files.
2. **Do not copy credentials from `gitIgnore/`:** The `gitIgnore/` folder contains historical local test files and scripts with placeholder or exposed secrets. Never replicate patterns or token strings from those files.
3. **Request verification timing safety:** When comparing HMAC signatures in `RequestVerification.cs`, use `System.Security.Cryptography.CryptographicOperations.FixedTimeEquals` to prevent side-channel timing attacks.

---

## 6. Known Anti-Patterns — Do Not Replicate

For full analysis, see `Documentation/ProjectAnalysis.md`. When modifying or refactoring existing code, avoid these patterns:

1. **Sync-over-Async (`.Wait()`, `.Result`):**
   - Do **not** call `.Wait()` or `.Result` on tasks. Make methods `async Task` and `await` properly.
   - Do not block `ConnectAsync` with synchronous waiting loops in WebSocket/Socket Mode code.
2. **Static `HttpClient`:**
   - Avoid adding more static state to `SlackClient`. Favor `IHttpClientFactory` or instance-based clients where possible.
3. **Duplicating files between projects:**
   - Classes should not be copied verbatim between `Blacus.SlackAPIWrapper` and `Blacus.SlackAPIWrapper.AspNetCore`. ASP.NET Core project should reference and extend the core library.
4. **`BuildServiceProvider()` inside DI extensions:**
   - Do not call `services.BuildServiceProvider()` during service configuration.

---

## 7. How to Add a New Slack Method (Step-by-Step)

1. Navigate to `src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/Blacus/SlackAPIWrapper/Methods/`.
2. Create a new file named after the method (e.g. `ConversationsArchive.cs`).
3. Decorate the class with `[SlackMethod("conversations.archive", "POST", SlackMethodContentType.ApplicationJson)]`.
4. Inherit from `SlackModel` (gives you `Ok`, `Error`, `Warning`, `SourceJson`).
5. Add response properties matching the Slack API docs using `[JsonPropertyName("...")]`.
6. Add a nested `public class OptionalArguments : SlackOptionalArguments` with request properties.
7. Verify compilation: `dotnet build src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/SlackAPIWrapper.csproj`.
8. Record changes in `Documentation/AI_CHANGELOG.md`.
