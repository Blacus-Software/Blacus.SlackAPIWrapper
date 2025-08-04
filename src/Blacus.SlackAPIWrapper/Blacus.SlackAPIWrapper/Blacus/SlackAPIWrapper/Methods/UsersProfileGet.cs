using Blacus.SlackAPIWrapper.Attributes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("users.profile.get", "Get", SlackMethodContentType.FormUrlEncoded)]
    public class UsersProfileGet : SlackMethod
    {

        public static async Task<UsersProfileGet> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<UsersProfileGet>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("user")]
            public string? User { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("include_labels")]
            public bool? IncludeLabels { get; set; }

        }

    }

}
