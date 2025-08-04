
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8604 // Possible null reference argument.

namespace Blacus.SlackAPIWrapper
{

    /// <summary>
    /// The main class for interacting with the Slack API.
    /// </summary>
    public partial class SlackClient
    {

        /// <summary>
        /// A shared instance of HttpClient for making requests to the Slack API.
        /// </summary>
        public static HttpClient HttpClient = new();

        /// <summary>
        /// Makes a request to the Slack API with the specified token and optional arguments.
        /// </summary>
        /// <typeparam name="T">The type of data to deserialize from the response.</typeparam>
        /// <param name="token">The authentication token for the request.</param>
        /// <param name="optionalArguments">Optional arguments for the request (e.g. JSON payload).</param>
        /// <returns>The deserialized response data.</returns>
        public static async Task<T> HttpRequestAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null)
        {

            var attribsMethod = SlackMethodAttribute.GetSlackMethodAttribute<T>();
            var attribLimiter = SlackLimiterTierAttribute.GetSlackLimiterAttribute<T>();

            if (attribLimiter != null)
                Limiter.LimiterFactory(attribLimiter.LimiterTier).Wait();

            var client = SlackClient.HttpClient;

            var request = new HttpRequestMessage(attribsMethod.HttpMethod, $"https://slack.com/api/{attribsMethod.Path}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            switch (attribsMethod.ContentType)
            {
                case SlackMethodContentType.Json:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    throw new NotImplementedException("JSON content type is not implemented yet.");

                case SlackMethodContentType.FormUrlEncoded:
                    if (optionalArguments != null)
                    {
                        var content = new FormUrlEncodedContent(optionalArguments?.GetFormUrlEncode());
                        request.Content = content;
                    }
                    break;

            }

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            T toReturn = System.Text.Json.JsonSerializer.Deserialize<T>(json, JsonConverters.JsonConverterSettings.Singleton);

            return toReturn;

        }

    }

}
