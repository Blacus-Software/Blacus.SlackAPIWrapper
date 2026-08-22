# AI Changelog

All changes made to this repository by AI agents must be recorded in this file in reverse chronological order.

---

## [Unreleased]

### Changed
- Merged the `Blacus.SlackAPIWrapper.AspNetCore` project into the core `Blacus.SlackAPIWrapper` project (it was 100% duplicate code); removed the now-redundant `Blacus.SlackAPIWrapper.AspNetCore/` directory and its entry from the `.sln`.
- Resolved the unresolved merge-conflict markers in `Blacus.SlackAPIWrapper.sln`, leaving a clean solution that references only the core `SlackAPIWrapper` project (also dropped the `gitIgnore/ConsoleAppSlackTestApp` entry flagged as an anti-pattern).
- Bumped the package `Version` from `9.1.2-beta` to `10.2.1`.
- Added `<Description>`, `<PackageLicenseExpression>MIT</PackageLicenseExpression>`, and `<PackageTags>` to `SlackAPIWrapper.csproj`.

### Added
- Created `.github/workflows/publish-nuget.yml`: on push to `main`, builds `Blacus.SlackAPIWrapper.csproj` in Release with the .NET 10 SDK, uploads the resulting nupkg as an artifact, and pushes it to nuget.org using the `NUGET_API_KEY` secret (`--skip-duplicate`).
- Created `AGENTS.md`, `CLAUDE.md`, `.cursorrules`, and `.github/copilot-instructions.md` containing developer and AI agent guidance, coding standards, build instructions, and security guidelines.
- Created `Documentation/AI_CHANGELOG.md` to track all AI-assisted repository modifications.
