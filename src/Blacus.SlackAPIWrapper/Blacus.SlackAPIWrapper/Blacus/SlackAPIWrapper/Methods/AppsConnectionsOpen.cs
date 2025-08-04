using Blacus.SlackAPIWrapper.Attributes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("apps.connections.open", "POST", SlackMethodContentType.FormUrlEncoded)]
    [SlackLimiterTier(LimiterTier.WebAPITier3)]
    public class AppsConnectionsOpen : SlackMethod
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        public class OptionalArguments : SlackOptionalArguments { }

        public static async Task<AppsConnectionsOpen> ExecuteAsync(string token)
              => await SlackClient.HttpRequestAsync<AppsConnectionsOpen>(token);

    }

}

