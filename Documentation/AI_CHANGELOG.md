# AI Changelog

All changes made to this repository by AI agents must be recorded in this file in reverse chronological order.

---

## [Unreleased]

### Changed
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
