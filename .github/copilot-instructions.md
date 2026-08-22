# GitHub Copilot Instructions for Blacus.SlackAPIWrapper

Please follow the rules and project guidelines defined in [AGENTS.md](../AGENTS.md).

Key guidelines:
- .NET 10 (`net10.0`) library with explicit namespaces (`<ImplicitUsings>disable</ImplicitUsings>`).
- Use `System.Text.Json` attributes (`[JsonPropertyName]`, `[JsonExtensionData]`).
- Do not introduce `Newtonsoft.Json`.
- Always verify code via `dotnet build src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/SlackAPIWrapper.csproj` and `dotnet build src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper.AspNetCore/SlackAPIWrapper.AspNetCore.csproj`.
- Never expose or commit tokens/secrets.
- Record any AI-made repository modifications in `Documentation/AI_CHANGELOG.md`.
