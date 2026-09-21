# AI Changelog

All changes made to this repository by AI agents must be recorded in this file in reverse chronological order.

---

## [Unreleased]

### Added
- Added `SlackTransmissionHandler` (`SlackTransmissionHandler.cs`):
  - Provides a static `HandleAsync(ISlackASPTransmission, Func<ISlackASPTransmission, Task<string?>>)` method that executes `transmission.PrepareAsync()` before invoking custom handler code that returns a response string.
  - Added unit test suite in `SlackTransmissionHandlerTests.cs` validating `PrepareAsync()` invocation, payload availability, return values, and argument null validation.
  - Updated `README.md` with usage example for `SlackTransmissionHandler.HandleAsync`.
- Added automatic `services.AddHttpContextAccessor()` registration to `AddSlackDependency` (`SlackDependencyExtensions.cs`) and added unit test in `DependencyInjectionTests.cs`.

### Changed
- Modernized `OAuthV2Access.OptionalArguments` (`OAuthV2Access.cs`):
  - Nested `OptionalArguments` class inside `OAuthV2Access` conforming to library-wide pattern.
  - Replaced `snake_case` properties with standard C# PascalCase properties decorated with `[JsonPropertyName]` and null-ignoring serialization: `ClientId`, `ClientSecret`, `Code`, `CodeVerifier`, `RedirectUri`, `GrantType`, and `RefreshToken`.
  - Maintained 100% backward compatibility via obsolete lowercase aliases (`client_id`, `client_secret`, `code`, `grant_type`, `redirect_uri`, `refresh_token`) and a top-level `OptionalArguments` type alias.
  - Updated `OAuthV2Access.ExecuteAsync` overloads to support Basic authentication header creation from `ClientId`/`ClientSecret`, bearer token requests, and `CancellationToken` propagation.
- Exposed virtual `ToJson()`, `GetFormUrlEncode()`, and `GetRequestQuery()` on `SlackOptionalArguments` base class backed by `SlackOptionalArgumentsHelper` for direct caller access without casting.

### Added
- Added `UserId`, `AppId`, and `IsEnterpriseInstall` response properties on `OAuthV2UserAccess` (`OAuthV2UserAccess.cs`).
- Added unit tests in `SlackMethodsTests.cs` for `OAuthV2Access.OptionalArguments`:
  - Verified form URL encoding generates expected Slack API `snake_case` keys.
  - Verified backward compatibility aliases and top-level type alias map correctly.
  - Verified `OAuthV2Access.ExecuteAsync` populates Basic authentication header and parameters.
- Added `LICENSE` file with standard MIT license text in repository root and packed into NuGet output.
- Added `.gitattributes` file to enforce consistent line endings across platforms and eliminate phantom CRLF/LF working directory diffs.
- Added `BotToken` and `AppLevelToken` convenience property aliases on `SlackCredential` to ensure code samples and consumer configuration bind cleanly.
- Enabled XML documentation generation (`<GenerateDocumentationFile>true</GenerateDocumentationFile>`) on `SlackAPIWrapper.csproj`.
- Added SourceLink and symbol packaging (`<IncludeSymbols>true</IncludeSymbols>`, `<SymbolPackageFormat>snupkg</SymbolPackageFormat>`, `Microsoft.SourceLink.GitHub 10.0.401`).
- Added unit tests in `DependencyInjectionTests.cs` validating `SlackCredential` property aliases.

### Changed
- Replaced legacy `Microsoft.AspNetCore.Http.Abstractions 2.3.0` and bloated `Microsoft.Extensions.*` package references with `<FrameworkReference Include="Microsoft.AspNetCore.App" />` in both library and test projects.
- Guarded `AllowOutOfOrderMetadataProperties` with `#if NET9_0_OR_GREATER` in `JsonConverterSettings.cs` to support `net8.0`.
- Typed `SlackASPTransmission.HttpContext` as nullable (`HttpContext?`) with explicit null guard in `PrepareAsync` to ensure clean nullability across all target frameworks.
- Updated `README.md` with License section and verified code samples.
- Cleaned up stale reference to `ProjectAnalysis.md` in `AGENTS.md`.

### Fixed
- Fixed and hardened GitHub Actions NuGet publishing workflow (`.github/workflows/publish-nuget.yml`):
  - Added `workflow_dispatch` trigger to enable manual on-demand publishing from the GitHub Actions UI.
  - Added `tags: ['v*']` and `release: [published]` triggers for automated publishing on tag pushes and GitHub releases.
  - Aligned .NET SDK setup to install 8.0, 9.0, and 10.0 matching multi-targeting requirements.
  - Updated NuGet source to canonical URL `https://api.nuget.org/v3/index.json`.
  - Added secret validation step with clear error messaging if `NUGET_API_KEY` is not configured in repository secrets.

### Added
- Added `oauth.v2.user.access` API method (`OAuthV2UserAccess.cs`):
  - Decorated with `[SlackMethod("oauth.v2.user.access", "Post", SlackMethodContentType.FormUrlEncoded)]`.
  - Added response properties: `AccessToken`, `TokenType`, `IdToken`, `Scope`, `AuthedUser`, `Team`, `Enterprise`, `ExpiresIn`, and `RefreshToken`.
  - Implemented `OptionalArguments` supporting `ClientId`, `ClientSecret`, `Code`, `CodeVerifier`, `RedirectUri`, `GrantType`, `RefreshToken`, and `Assertion`.
  - Added `ExecuteAsync` overloads supporting Basic authentication header encoding when `ClientId`/`ClientSecret` are provided, as well as token/PKCE requests.
- Added `views.publish` API method (`ViewsPublish.cs`):
  - Decorated with `[SlackMethod("views.publish", "Post", SlackMethodContentType.Json)]` and `[SlackLimiterTier(LimiterTier.WebAPITier4)]`.
  - Added response `View` property.
  - Implemented `OptionalArguments` supporting `UserId`, `View`, `Hash`, and `InteractivityPointer`.
  - Added static helper `ExecuteAsync`.
- Implemented payload properties on `EventAppHomeOpened.cs` for `app_home_opened` events:
  - `Type` (with default `"app_home_opened"` and `[JsonIgnore]` to avoid conflict with polymorphic discriminator metadata property)
  - `User` (`[JsonPropertyName("user")]`)
  - `Channel` (`[JsonPropertyName("channel")]`)
  - `Tab` (`[JsonPropertyName("tab")]`)
  - `View` (`[JsonPropertyName("view")]`)
  - `EventTs` (`[JsonPropertyName("event_ts")]`)
- Added unit test in `PolymorphicSerializationTests.cs` verifying polymorphic deserialization of `EventAppHomeOpened` from JSON payload.

### Fixed
- Fixed critical runtime `NullReferenceException` bugs in property getters/setters:
  - Added null check in `TextObject.Text` getter so null values with `MaxTextLength` return `null` instead of throwing when accessing string length.
  - Added null guards in `ConfirmationDialogObject.Text`, `DatePickerElement.Placeholder`, and `BlockOptionObject.Text` setters before accessing `MaxTextLength`.
  - Added null guard in `JsonExtensionDataHelper` constructor when `slackModel.ExtensionData` is null, and properly typed `Model` property as nullable (`ISlackModel?`).
  - Added null check in `SlackASPTransmission.PrepareAsync` when passing `urlPayload.Challenge` to `UrlVerificationResponse`.
- Eliminated all global CS86xx suppressions in `SlackAPIWrapper.csproj` (`NoWarn`) and local `#pragma warning disable` blocks:
  - Corrected return types of `SlackMethodAttribute.GetSlackMethodAttribute<T>()` and `SlackLimiterTierAttribute.GetSlackLimiterAttribute<T>()` to nullable.
  - Properly annotated nullable response fields and request parameters in `ConversationsHistory.cs`.
  - Removed `#pragma` directives from `SlackMethodAttribute.cs`, `SlackLimiterTierAttribute.cs`, `BlockOptionObject.cs`, `ChatPostMessage.cs`, and `ConversationsHistory.cs`.
- Fixed polymorphic deserialization on abstract `SlackWebSocketTransmission` by adding `[JsonPolymorphic]` and `[JsonDerivedType]` attributes matching `ISlackWebSocketBase`.
- Added missing static `ExecuteAsync` helper methods to `ConversationsOpen.cs` and `ConversationsReplies.cs` for consistent API surface.
- Removed orphaned dead code method `ChatPostMessage.TransmitAsync`.

### Added
- Expanded automated xUnit test suite from 41 to 68 tests:
  - Added comprehensive test coverage for `SlackASPTransmission.PrepareAsync` (URL challenge handling, valid signatures, expired timestamps, invalid signatures returning 401, idempotency).
  - Added unit tests for null safety guards across `TextObject`, `ConfirmationDialogObject`, `DatePickerElement`, and `BlockOptionObject`.
  - Added unit tests for `JsonExtensionDataHelper` with null and populated extension data.
  - Added unit tests for `SlackWebSocketTransmission.FromJson` polymorphic deserialization.
  - Added unit tests for `SlackMethodAttribute`, `SlackLimiterTierAttribute`, `SlackOptionalArguments`, and static `ExecuteAsync` methods.

### Fixed
- Fixed critical bug in `DateOnlyConverter.cs`: corrected `CanConvert(Type t)` to check `t == typeof(DateOnly)` instead of `typeof(TimeOnly)`; optimized writing directly to `Utf8JsonWriter`.
- Enhanced `TimeOnlyConverter.cs` and `BoolConverter.cs`: optimized writer output; added numeric (`0`/`1`) and string boolean support in `BoolConverter`; replaced generic exceptions with `JsonException`.
- Enhanced `DateTimeOffsetAsIntConverter.cs` to support both numeric and string-encoded Unix timestamps (e.g. `"1512085950.000216"` common in Slack message `ts` values).
- Fixed `SlackWebSocketClient` outbound send race condition: protected `SendMessageAsync` and disconnect close handshake with a `SemaphoreSlim` to eliminate `InvalidOperationException` concurrent send crashes.
- Guarded `ChatPostMessage.ExecuteAsync` against `ArgumentNullException` when `Channel` is null or empty.
- Replaced anti-pattern `throw new NullReferenceException()` with `ArgumentException` and `ArgumentNullException` in `ISlackModel.cs` and `SlackWebSocketTransmission.cs`.
- Refactored `SlackWebSocketTransmission` derived classes (`SlackWebSocketTransmissionSlashCommand`, `SlackWebSocketTransmissionInteractive`, `SlackWebSocketTransmissionEventsApi`, `SlackWebSocketTransmissionDisconnect`, `SlackWebSocketTransmissionHello`) to inherit from `SlackWebSocketTransmission` and eliminated brittle reflection in `ISlackWebSocketPayload.GetPayload()`.
- Removed `Console.WriteLine` side effect in `SlackASPTransmission.cs` and cleaned up fallback parsing.

### Changed
- Added `CancellationToken cancellationToken = default` to `ISlackClient.SendAsync<T>` and `SlackClient.SendAsync<T>`.
- Modernized `RequestVerification.cs`: upgraded timestamp handling to 64-bit `long` to protect against 2038 overflow, used `Convert.ToHexStringLower` under .NET 9+, and hardened signature/secret validation.

### Added
- Added automated xUnit test suite in `tests/Blacus.SlackAPIWrapper.Tests/` covering JSON converters, HMAC request verification, rate limiting, dependency injection, polymorphic serialization, and client dispatching.
- Added GitHub Actions continuous integration workflow `.github/workflows/ci.yml` to automatically build and run tests on pull requests and pushes across .NET 8, 9, and 10.
- Cleaned up dead code: deleted `JsonConverters/EventConverter.cs`, which was 100% commented out and had been superseded by System.Text.Json native polymorphic attributes (`[JsonPolymorphic]`, `[JsonDerivedType]`) on `ISlackPayloadEvent`. Updated `Documentation/ProjectAnalysis.md` accordingly.
- Fixed fragile form parsing in `SlackASPTransmission.cs`: replaced manual string splitting on `&` and `=` (which threw `KeyNotFoundException` / `IndexOutOfRangeException` on missing or malformed fields) with robust `HttpUtility.ParseQueryString`, safe `Uri.TryCreate` for `ResponseUrl`, and safe parsing of optional fields (`TeamDomain`, `ChannelName`, `IsEnterpriseInstall`). Removed the fragile form parsing item from `Documentation/ProjectAnalysis.md`.
- Hardened WebSocket lifecycle in `SlackWebSocketClient.cs`: verified non-blocking `ConnectAsync` (starts background loop with `Task.Run` and does not call `.Wait()`), eliminated sync-over-async blocking in synchronous `Dispose()`, added handshake response validation for `AppsConnectionsOpen`, improved graceful socket closure and receive loop cancellation during `DisconnectAsync()`, and removed dead code in payload processing. Marked §5.2 and P1 roadmap item in `Documentation/ProjectAnalysis.md` as resolved.
- Refactored `SlackClient.cs` for dependency injection and `IHttpClientFactory` support: introduced `ISlackClient` interface, added instance constructors accepting `HttpClient` or `IHttpClientFactory`, wired typed client registration via `services.AddHttpClient<ISlackClient, SlackClient>()` in `SlackDependencyExtensions.cs`, and deduplicated internal `HttpRequestAsync<T>` request dispatching. Preserved static methods and `HttpClient` for backward compatibility. Removed §5.3.1 from `Documentation/ProjectAnalysis.md` and marked the P2 roadmap row as resolved.
- Hardened dependency injection registration in `SlackDependencyExtensions.cs`: added null checks for `IServiceCollection` and `SlackCredential`, registered `SlackCredential` as a singleton in DI, and switched `IHttpContextAccessor` resolution to `sp.GetRequiredService<IHttpContextAccessor>()` for explicit error reporting when the host has not registered accessor services. Removed stale §5.3.2 entry in `Documentation/ProjectAnalysis.md` (which falsely claimed `services.BuildServiceProvider()` was being called).
- Cleaned up all stale references to the former `Blacus.SlackAPIWrapper.AspNetCore` project following its merge into the core library: removed references to the secondary `.csproj` from `AGENTS.md` (project overview, build/clean commands, anti-patterns), `README.md` (features and project structure sections), `.cursorrules`, `.github/copilot-instructions.md`, and `Documentation/ProjectAnalysis.md` (executive summary, directory tree, §5.3.2 "Code Duplication Across Projects" item, and roadmap P1 row).
- Rewrote `Limiter.cs` to fix its thread-safety and lifecycle defects: single-instance construction now uses a `Lazy<Limiter>` (`ExecutionAndPublication`) registry, eliminating the previous `ConcurrentDictionary.GetOrAdd` double-construction race that leaked live `System.Timers.Timer` instances; the refill loop is driven by `System.Threading.PeriodicTimer` on a background task (no `lock`/`Enabled` dance); `Limiter` is now `sealed`, implements `IAsyncDisposable` (cancels the refill loop, disposes the timer/semaphore, and self-removes from the registry), and `WaitAsync` accepts a `CancellationToken`. Added `LimiterFactory.TryGet`, `RemoveAsync(id)`, and `DisposeAllAsync` to release the previously unbounded per-identifier limiters. The blocking `Wait()` was removed (no in-repo callers) and `NullReferenceException` on bad constructor args was replaced with `ArgumentNullException`/`ArgumentOutOfRangeException`.
- Removed the sync-over-async / constructor I/O in `SlackASPTransmission.cs`: the constructor now only captures request state; all I/O moved to a new `PrepareAsync()` which reads the request body with `StreamReader.ReadToEndAsync()`, writes the URL-verification challenge with `Response.WriteAsync()` (`Content-Type: application/json`), and sets `Response.StatusCode = 401` for failed verification. `ISlackASPTransmission` gained `PrepareAsync()`, `Verified`, and `ShortCircuited`; `Payload` throws `InvalidOperationException` if accessed before `PrepareAsync()`.
- Hardened `RequestVerification.cs` timestamp handling: replaced `int.Parse(SlackRequestTimestamp)` (which threw `FormatException` on malformed input) with `int.TryParse`-backed `SlackRequestTimestampInt` / `HasValidTimestamp`; an unparseable or missing timestamp now fails verification cleanly instead of throwing.
- Verified `RequestVerification.cs` already uses `System.Security.Cryptography.CryptographicOperations.FixedTimeEquals` for the signature comparison (constant-time, returns `false` on length mismatch) for the requested timing-attack fix; no code change was required. Removed the stale P0 entry in `Documentation/ProjectAnalysis.md` (formerly described a non-existent `StringComparer.OrdinalIgnoreCase.Compare`-based comparison).

### Changed
- Comprehensively updated and aligned repository documentation: updated `README.md` with accurate API execution patterns (`ISlackClient.SendAsync<T>`, `AddSlackDependency`), corrected Socket Mode description, fixed method attribute signatures; updated `AGENTS.md` code examples (`SlackMethodContentType.Json`, `LimiterTier.WebAPITierX`); updated `Documentation/ProjectAnalysis.md` directory layout, architecture, and marked P3 "Update Documentation & Clean Dead Code" as resolved.
- Adopted .NET 10 / C# 14 as the primary target: added `<LangVersion>14</LangVersion>` and documented `net10.0` / C# 14 in `AGENTS.md`, `CLAUDE.md`, `.cursorrules`, `.github/copilot-instructions.md`, and `Documentation/ProjectAnalysis.md`.
- Refactored backing-field accessors to use the C# 14 `field` keyword in `TextObject.cs`, `BlockOptionObject.cs`, `DatePickerElement.cs`, and `ConfirmationDialogObject.cs` (removed the private backing fields `_text`, `placeholder`, `text`, `_emoji`); renamed their length constants to the `field_MaxTextLength` / `field_MaxPlaceholderLength` naming convention.
- Updated `Documentation/ProjectAnalysis.md` to reflect the current `10.2.1` version and removed the stale "Version Mismatch" anti-pattern note (now accurate after the AspNetCore merge).
- Merged the `Blacus.SlackAPIWrapper.AspNetCore` project into the core `Blacus.SlackAPIWrapper` project (it was 100% duplicate code); removed the now-redundant `Blacus.SlackAPIWrapper.AspNetCore/` directory and its entry from the `.sln`.
- Resolved the unresolved merge-conflict markers in `Blacus.SlackAPIWrapper.sln`, leaving a clean solution that references only the core `SlackAPIWrapper` project (also dropped the `gitIgnore/ConsoleAppSlackTestApp` entry flagged as an anti-pattern).
- Bumped the package `Version` from `9.1.2-beta` to `10.2.1`.
- Added `<Description>`, `<PackageLicenseExpression>MIT</PackageLicenseExpression>`, and `<PackageTags>` to `SlackAPIWrapper.csproj`.

### Added
- Created `.github/workflows/publish-nuget.yml`: on push to `main`, builds `Blacus.SlackAPIWrapper.csproj` in Release with the .NET 10 SDK, uploads the resulting nupkg as an artifact, and pushes it to nuget.org using the `NUGET_API_KEY` secret (`--skip-duplicate`).
- Created `AGENTS.md`, `CLAUDE.md`, `.cursorrules`, and `.github/copilot-instructions.md` containing developer and AI agent guidance, coding standards, build instructions, and security guidelines.
- Created `Documentation/AI_CHANGELOG.md` to track all AI-assisted repository modifications.
