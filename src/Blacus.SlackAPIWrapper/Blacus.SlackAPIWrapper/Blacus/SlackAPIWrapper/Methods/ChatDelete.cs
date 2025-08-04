using Blacus.SlackAPIWrapper.Attributes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("chat.delete", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ChatDelete : SlackMethod
    {

        public static async Task<ChatDelete> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<ChatDelete>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("channel")]
        public string? Channel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("ts")]
        public string? Ts { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            [JsonPropertyName("channel")]
            public required string Channel { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            [JsonPropertyName("ts")]
            public required string Ts { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            [JsonPropertyName("as_user")]
            public bool? AsUser { get; set; }

        }

    }

}
