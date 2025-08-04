using Blacus.SlackAPIWrapper.Attributes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("assistant.threads.setTitle", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class AssistantThreadsSetTitle : SlackMethod
    {

        public static async Task<AssistantThreadsSetTitle> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<AssistantThreadsSetTitle>(token, slackOptionalArguments);

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("channel_id")]
            public string? ChannelId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("thread_ts")]
            public string? ThreadTs { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("title")]
            public string? Title { get; set; }

        }

    }

}