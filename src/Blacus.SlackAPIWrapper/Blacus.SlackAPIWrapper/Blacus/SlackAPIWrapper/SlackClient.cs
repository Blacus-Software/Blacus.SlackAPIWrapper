
using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{

    public partial class SlackClient
    {

        public static async Task<T> MakeHttpRequestAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null)
        {

            var attribsMethod = SlackMethodAttribute.GetSlackMethodAttribute<T>();
            var attribLimiter = SlackLimiterTierAttribute.GetSlackLimiterAttribute<T>();

            if (attribLimiter != null)
                Limiter.LimiterFactory(attribLimiter.LimiterTier).Wait();

            var client = new HttpClient();

            var request = new HttpRequestMessage(attribsMethod.HttpMethod, $"https://slack.com/api/{attribsMethod.Path}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            switch (attribsMethod.ContentType)
            {
                case SlackMethodContentType.Json:
                    request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    throw new NotImplementedException("JSON content type is not implemented yet.");
                    break;

                case SlackMethodContentType.FormUrlEncoded:
                    if (optionalArguments != null)
                    {
                        var content = new FormUrlEncodedContent(optionalArguments?.GetFormUrlEncode());
                        request.Content = content;
                    }
                    break;

                case SlackMethodContentType.MultipartFormData:
                    throw new NotImplementedException("Multipart form data is not implemented.");

            }

            var response = await client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            T toReturn = System.Text.Json.JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton);

            return toReturn;

        }

    }

}
