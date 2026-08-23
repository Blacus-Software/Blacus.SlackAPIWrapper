using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{

    /// <summary>
    /// The main class for interacting with the Slack API.
    /// Supports both dependency injection (via <see cref="ISlackClient"/> / <see cref="IHttpClientFactory"/>)
    /// and static method invocations for backward compatibility.
    /// </summary>
    public partial class SlackClient : ISlackClient
    {

        /// <summary>
        /// A shared instance of HttpClient used by static calls when dependency injection is not used.
        /// </summary>
        public static HttpClient HttpClient { get; set; } = new();

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of <see cref="SlackClient"/> with an optional <see cref="HttpClient"/>.
        /// If none is provided, the static shared <see cref="HttpClient"/> instance is used.
        /// </summary>
        public SlackClient(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? HttpClient;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="SlackClient"/> using <see cref="IHttpClientFactory"/>.
        /// </summary>
        public SlackClient(IHttpClientFactory httpClientFactory)
        {
            if (httpClientFactory == null)
                throw new ArgumentNullException(nameof(httpClientFactory));

            _httpClient = httpClientFactory.CreateClient(nameof(SlackClient));
        }

        /// <summary>
        /// Sends an instance-based request to the Slack API with the specified Bearer token and optional arguments.
        /// </summary>
        public async Task<T> SendAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true) where T : SlackMethod
        {
            return await SendAsync<T>(new AuthenticationHeaderValue("Bearer", token), optionalArguments, streamJson);
        }

        /// <summary>
        /// Sends an instance-based request to the Slack API with the specified authentication header and optional arguments.
        /// </summary>
        public async Task<T> SendAsync<T>(AuthenticationHeaderValue authenticationHeaderValue, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true) where T : SlackMethod
        {

            var attribsMethod = SlackMethodAttribute.GetSlackMethodAttribute<T>();
            var attribLimiter = SlackLimiterTierAttribute.GetSlackLimiterAttribute<T>();

            if (attribLimiter != null)
                await Limiter.LimiterFactory(attribLimiter.LimiterTier).WaitAsync();

            var request = new HttpRequestMessage(attribsMethod.HttpMethod, $"https://slack.com/api/{attribsMethod.Path}");

            request.Headers.Authorization = authenticationHeaderValue;

            switch (attribsMethod.ContentType)
            {
                case SlackMethodContentType.Json:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var jsonText = optionalArguments?.ToJson();
                    var jsonContent = new StringContent(jsonText ?? string.Empty, null, "application/json");
                    request.Content = jsonContent;
                    break;

                case SlackMethodContentType.FormUrlEncoded:
                    if (optionalArguments != null)
                    {
                        var formUrlEncodedContent = new FormUrlEncodedContent(optionalArguments.GetFormUrlEncode());
                        request.Content = formUrlEncodedContent;
                    }
                    break;
            }

            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            T toReturn = null!;

            if (streamJson)
            {
                toReturn = (await ISlackModel.FromJsonAsync<T>(await response.Content.ReadAsStreamAsync()))!;
            }
            else
            {
                string json = await response.Content.ReadAsStringAsync();
                toReturn = ISlackModel.FromJson<T>(json)!;
            }

            return toReturn;

        }

        /// <summary>
        /// Makes a static request to the Slack API with the specified Bearer token and optional arguments.
        /// </summary>
        public static async Task<T> HttpRequestAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true) where T : SlackMethod
        {
            return await new SlackClient(HttpClient).SendAsync<T>(token, optionalArguments, streamJson);
        }

        /// <summary>
        /// Makes a static request to the Slack API with the specified authentication header and optional arguments.
        /// </summary>
        public static async Task<T> HttpRequestAsync<T>(AuthenticationHeaderValue authenticationHeaderValue, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true) where T : SlackMethod
        {
            return await new SlackClient(HttpClient).SendAsync<T>(authenticationHeaderValue, optionalArguments, streamJson);
        }

    }

}
