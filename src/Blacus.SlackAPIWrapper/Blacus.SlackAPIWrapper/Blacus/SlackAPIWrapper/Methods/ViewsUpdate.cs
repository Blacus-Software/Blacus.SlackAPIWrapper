using Blacus.SlackAPIWrapper.Attributes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("views.update", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ViewsUpdate : SlackMethod
    {

        public static async Task<ViewsUpdate> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<ViewsUpdate>(token, slackOptionalArguments);

        public string? external_id { get; set; }
        public string? view_id { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("view")]
            public string? View { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("external_id")]
            public string? ExternalId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("view_id")]
            public string? ViewId { get; set; }

        }

    }

}