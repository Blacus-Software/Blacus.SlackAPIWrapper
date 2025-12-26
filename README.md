# Blacus Slack API Wrapper

A modern, extensible, and easy-to-use .NET wrapper for the Slack API, designed with performance and scalability in mind.

## Overview

The Blacus.SlackAPIWrapper is a .NET library designed to simplify interactions with the Slack API. It provides a strongly-typed, attribute-based architecture that makes adding new Slack API methods straightforward. It handles authentication, rate limiting, and request/response serialization, allowing you to focus on building your application's logic.

## Core Features

*   **Declarative API Endpoints**: Simply decorate a class with a `[SlackMethod]` attribute to define a new API method. The wrapper handles the rest.
*   **Built-in Rate Limiting**: A sophisticated token-bucket rate limiter (`Limiter.cs`) helps you stay within Slack's API limits automatically.
*   **Strongly-Typed Models**: Comprehensive and strongly-typed C# classes for Slack API objects, requests, and responses, reducing runtime errors.
*   **Dependency Injection Ready**: Designed for modern .NET applications with seamless `HttpClientFactory` and DI container integration.
*   **ASP.NET Core Support**: A dedicated `Blacus.SlackAPIWrapper.AspNetCore` project provides extension methods for easy setup in web applications.
*   **WebSocket Client**: Includes a `SlackWebSocketClient` for connecting to Slack's Real-Time Messaging (RTM) API.

## Architecture

The wrapper's architecture is designed for simplicity and extensibility:

1.  **`SlackClient.cs`**: The heart of the library. This central client uses `HttpClient` to send requests. Its generic `HttpRequestAsync<T>()` method takes a method class `T` and handles the entire request lifecycle.

2.  **`[SlackMethodAttribute]`**: This attribute is used to declaratively configure an API method class with its corresponding API path, HTTP method (GET/POST), and content type.

3.  **Method Classes**: Each Slack API method (e.g., `chat.postMessage`) is represented by its own class (e.g., `ChatPostMessage.cs`).
    *   The class properties define the expected **response** fields from the Slack API.
    *   A nested `OptionalArguments` class defines the **request** parameters that can be sent to the API.

4.  **`Limiter.cs`**: A smart rate limiter that ensures your application behaves respectfully towards the Slack API, preventing 429 "Too Many Requests" errors.

## Getting Started

### 1. Configuration in ASP.NET Core

The easiest way to get started is by using the ASP.NET Core integration. In your `Program.cs` or `Startup.cs`, add the following:

```csharp
// In Program.cs
builder.Services.AddSlackAPIWrapper(options =>
{
    // Optionally configure SlackCredential if needed
    // options.AppLevelToken = "xapp-...";
});
```

This extension method registers the `SlackClient` and other necessary services with the dependency injection container and configures `HttpClientFactory`.

### 2. Executing an API Method

Inject `SlackClient` into your services or controllers and call the desired method.

```csharp
using Blacus.SlackAPIWrapper;
using Blacus.SlackAPIWrapper.Methods;

public class MySlackService
{
    private readonly SlackClient _slackClient;
    private readonly IConfiguration _configuration;

    public MySlackService(SlackClient slackClient, IConfiguration configuration)
    {
        _slackClient = slackClient;
        _configuration = configuration;
    }

    public async Task<bool> SayHello(string channel)
    {
        var token = _configuration["Slack:BotToken"]; // Store your token securely!

        var response = await _slackClient.ExecuteAsync(new ChatPostMessage(token, channel, "Hello, world!"));

        return response.Ok;
    }
}
```

## How to Add a New API Method

Adding a new API endpoint is simple:

1.  Create a new class for the method (e.g., `UsersInfo.cs`).
2.  Add the `[SlackMethod]` attribute, specifying the endpoint path.
3.  Define properties on the class for the fields you expect in the API response. You can inherit from `SlackModel` for base properties like `Ok`.
4.  Create a nested public class called `OptionalArguments` that inherits from `SlackOptionalArguments`.
5.  Add properties to `OptionalArguments` for each parameter the API method accepts.

**Example: `UsersInfo.cs`**
```csharp
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Methods
{
    [SlackMethod("users.info")]
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

## Project Structure

*   `src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper`: The core library containing the `SlackClient`, method definitions, models, and rate limiter.
*   `src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper.AspNetCore`: Provides ASP.NET Core integration, primarily the `AddSlackAPIWrapper` extension method.

## Contributing

Contributions are welcome! If you'd like to report a bug, suggest an improvement, or add a new feature, please open an issue or submit a pull request on the project's GitHub page.
