using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("conversations.info", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ConversationsInfo : SlackMethod
    {

        public static async Task<ConversationsInfo> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<ConversationsInfo>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel")]
        public Channel? Channel { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("channel")]
            public string? Channel { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("include_locale")]
            public string? IncludeLocale { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("include_num_members")]
            public string? IncludeNumMembers { get; set; }

        }

    }

}