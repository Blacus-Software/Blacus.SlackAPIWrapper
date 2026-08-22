using Blacus.SlackAPIWrapper.Attributes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("assistant.threads.setStatus", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class AssistantThreadsSetStatus : SlackMethod
    {

        public static async Task<AssistantThreadsSetStatus> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<AssistantThreadsSetStatus>(token, slackOptionalArguments);

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("channel_id")]
            public string? ChannelId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("thread_ts")]
            public string? ThreadTs { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("status")]
            public string? Status { get; set; }

        }

    }

}