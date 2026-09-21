# Blacus Slack API Wrapper

A modern, extensible, and easy-to-use .NET wrapper for the Slack API, designed with performance and scalability in mind.

## Overview

`Blacus.SlackAPIWrapper` is a .NET 8 / 9 / 10 library designed to simplify interactions with the Slack API. It provides a strongly-typed, attribute-driven architecture that makes adding and invoking Slack API methods straightforward. It handles authentication, rate limiting, and request/response serialization, allowing you to focus on building your application's logic.

## Core Features

*   **Declarative API Endpoints**: Simply decorate a class with a `[SlackMethod]` attribute to define a new API method. The wrapper handles request and response serialization.
*   **Built-in Rate Limiting**: An async token-bucket rate limiter (`Limiter.cs`) driven by `PeriodicTimer` helps you stay within Slack's API tier limits automatically.
*   **Strongly-Typed Models**: Comprehensive and strongly-typed C# classes for Slack API objects, Block Kit components, events, and payloads.
*   **Dependency Injection & `IHttpClientFactory`**: First-class support for DI via `ISlackClient` and `IHttpClientFactory` connection pooling.
*   **ASP.NET Core Integration**: Built-in request verification (`RequestVerification`), webhook handling (`SlackASPTransmission`), and DI extensions (`AddSlackDependency`).
*   **Slack Socket Mode Client**: Includes a non-blocking `SlackWebSocketClient` for connecting to Slack's Socket Mode WebSocket API.

## Architecture

1.  **`SlackClient.cs` / `ISlackClient.cs`**: The core HTTP client. It supports constructor injection via `IHttpClientFactory` / `HttpClient` (`ISlackClient.SendAsync<T>()`) as well as static calls (`SlackClient.HttpRequestAsync<T>()`) for backward compatibility.
2.  **`[SlackMethodAttribute]`**: Configures an API method class with its corresponding API path, HTTP method (GET/POST), and content type (JSON or FormUrlEncoded).
3.  **Method Classes**: Each Slack API method (e.g., `chat.postMessage`) is represented by its own class (e.g., `ChatPostMessage.cs`).
    *   Class properties define expected **response** fields.
    *   A nested `OptionalArguments` class defines **request** parameters.
4.  **`Limiter.cs`**: Enforces Slack's tier-based rate limits (Tier 1–4) and per-channel limits, preventing HTTP 429 "Too Many Requests" errors.

## Getting Started

### 1. Configuration in ASP.NET Core

In your `Program.cs`, register the Slack services using `AddSlackDependency`:

```csharp
using Blacus.SlackAPIWrapper;

var builder = WebApplication.CreateBuilder(args);

// Register ISlackClient, SlackClient, and ASP.NET Core webhook integration
builder.Services.AddSlackDependency(new SlackCredential
{
    BotToken = builder.Configuration["Slack:BotToken"],
    SigningSecret = builder.Configuration["Slack:SigningSecret"],
    AppLevelToken = builder.Configuration["Slack:AppLevelToken"]
});
```

### 2. Executing an API Method

Inject `ISlackClient` into your services or controllers and send API requests:

```csharp
using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Methods;
using System.Threading.Tasks;

public class MySlackService
{
    private readonly ISlackClient _slackClient;
    private readonly string _botToken;

    public MySlackService(ISlackClient slackClient, IConfiguration configuration)
    {
        _slackClient = slackClient;
        _botToken = configuration["Slack:BotToken"]!;
    }

    public async Task<bool> PostGreetingAsync(string channelId, string message)
    {
        var parameters = new ChatPostMessage.OptionalArguments
        {
            Channel = channelId,
            Text = message
        };

        var response = await _slackClient.SendAsync<ChatPostMessage>(_botToken, parameters);

        return response.Ok;
    }
}
```

You can also use standalone static methods directly:

```csharp
var response = await ChatPostMessage.ExecuteAsync(botToken, new ChatPostMessage.OptionalArguments
{
    Channel = "C12345678",
    Text = "Hello from Blacus.SlackAPIWrapper!"
});
```

### 3. Handling Webhooks & Events in ASP.NET Core

You can handle incoming webhooks and events cleanly using `SlackTransmissionHandler`:

```csharp
app.MapPost("/slack/events", async (ISlackASPTransmission transmission) =>
{
    return await SlackTransmissionHandler.HandleAsync(transmission, async (t) =>
    {
        var payload = t.Payload;
        // Process verified Slack event or slash command and return a response string
        return "OK";
    });
});
```

Or manage preparation directly with `ISlackASPTransmission`:

```csharp
app.MapPost("/slack/events", async (ISlackASPTransmission transmission) =>
{
    await transmission.PrepareAsync();

    if (transmission.ShortCircuited)
    {
        // Challenge responses (URL verification) and 401 unverified requests are handled automatically
        return;
    }

    var payload = transmission.Payload;
    // Process verified Slack event or slash command
});
```

## How to Add a New API Method

1.  Create a new class for the endpoint (e.g., `UsersInfo.cs`).
2.  Add the `[SlackMethod]` attribute with the endpoint path, HTTP method, and content type.
3.  Inherit from `SlackModel` (provides `Ok`, `Error`, `Warning`, `SourceJson`).
4.  Add response properties matching the Slack API JSON documentation.
5.  Create a nested public class called `OptionalArguments` that inherits from `SlackOptionalArguments`.

**Example: `UsersInfo.cs`**
```csharp
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Methods
{
    [SlackMethod("users.info", "POST", SlackMethodContentType.Json)]
    public class UsersInfo : SlackModel
    {
        [JsonPropertyName("user")]
        public User? User { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {
            [JsonPropertyName("user")]
            public string? UserId { get; set; }

            [JsonPropertyName("include_locale")]
            public bool? IncludeLocale { get; set; }
        }
    }
}
```

## Project Structure

*   `src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper`: The unified library containing the HTTP client (`SlackClient`, `ISlackClient`), method definitions, models, converters, token-bucket rate limiter (`Limiter`), Socket Mode client (`SlackWebSocketClient`), and ASP.NET Core webhook verification (`SlackASPTransmission`, `RequestVerification`, `AddSlackDependency`).

## Contributing

Contributions are welcome! If you'd like to report a bug, suggest an improvement, or add a new feature, please open an issue or submit a pull request on GitHub.

## License

This project is licensed under the [MIT License](LICENSE).
