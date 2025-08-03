
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
        public SlackClient(SlackCredential slackCredential)
        {
            SlackCredential = slackCredential;
        }

        public SlackCredential SlackCredential { get; }

        public AppsConnectionsOpen AppsConnectionsOpen()
              => Methods.AppsConnectionsOpen.ExecuteAsync(SlackCredential.AppToken).Result;


        public static async Task<T> MakeHttpRequestAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null)
        {

            var attribs = SlackMethodAttribute.GetSlackMethodAttribute<T>();

            var client = new HttpClient();

            var request = new HttpRequestMessage(attribs.HttpMethod, $"https://slack.com/api/{attribs.Path}");

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            switch (attribs.ContentType)
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
