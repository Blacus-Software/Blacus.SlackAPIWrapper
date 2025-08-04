# Blacus Slack API Wrapper README

## Overview

The Blacus.SlackAPIWrapper is a .NET library designed to simplify interactions with the Slack API. This wrapper provides a convenient and user-friendly interface for making requests to the Slack API, handling authentication and rate limiting.

## Features

*   Makes requests to the Slack API with authentication token
*   Supports JSON payloads and form-encoded arguments
*   Handles rate limiting using the `Limiter` class
*   Uses `HttpClient` under the hood for efficient request handling

## Usage

To use this wrapper, simply create an instance of the `SlackClient` class and call the `HttpRequestAsync` method with your authentication token and optional arguments.

```csharp src/Blacus.SlackAPIWrapper/Blacus.SlackAPIWrapper/Blacus/SlackAPIWrapper/SlackClient.cs
public static async Task<T> HttpRequestAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null)
```

## Configuration

This wrapper uses the following configuration:

*   Authentication token: Provided as a `string` parameter in the `HttpRequestAsync` method
*   Optional arguments: Passed as an instance of `ISlackOptionalArguments` in the `HttpRequestAsync` method

### Limitations

The JSON content type is not implemented yet.

## Contributing

Contributions are welcome! If you'd like to report a bug or suggest improvements, please submit an issue on this repository's GitHub page.