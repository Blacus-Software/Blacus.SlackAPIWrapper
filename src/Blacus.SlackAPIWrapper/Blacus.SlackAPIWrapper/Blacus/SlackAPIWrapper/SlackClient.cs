
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
        public static async Task<T> HttpRequestAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true) where T : SlackMethod
        {

            var attribsMethod = SlackMethodAttribute.GetSlackMethodAttribute<T>();
            var attribLimiter = SlackLimiterTierAttribute.GetSlackLimiterAttribute<T>();

            if (attribLimiter != null)
                Limiter.LimiterFactory(attribLimiter.LimiterTier).Wait();

            var request = new HttpRequestMessage(attribsMethod.HttpMethod, $"https://slack.com/api/{attribsMethod.Path}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            switch (attribsMethod.ContentType)
            {
                case SlackMethodContentType.Json:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var jsonText = optionalArguments.ToJson();
                    var jsonContent = new StringContent(jsonText, null, "application/json");
                    request.Content = jsonContent;
                    break;

                case SlackMethodContentType.FormUrlEncoded:
                    if (optionalArguments != null)
                    {
                        var formUrlEncodedContent = new FormUrlEncodedContent(optionalArguments?.GetFormUrlEncode());
                        request.Content = formUrlEncodedContent;
                    }
                    break;

            }

            var response = await HttpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            T toReturn = null;

            if (streamJson)
            {
                toReturn = ISlackModel.FromJson<T>(response.Content.ReadAsStream());
            }
            else
            {
                string json = await response.Content.ReadAsStringAsync();

                toReturn = ISlackModel.FromJson<T>(json);
            }

            return toReturn;

        }

    }

}
