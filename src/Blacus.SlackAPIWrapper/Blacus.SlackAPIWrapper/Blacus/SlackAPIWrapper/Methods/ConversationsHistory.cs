using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("conversations.history", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ConversationsHistory : SlackMethod
    {

        public static async Task<ConversationsHistory> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.MakeHttpRequestAsync<ConversationsHistory>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("has_more")]
        public bool? HasMore { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("pin_count")]
        public long? PinCount { get; set; }

        [JsonPropertyName("channel_actions_ts")]
        public string ChannelActionsTs { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_actions_count")]
        public long? ChannelActionsCount { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("oldest")]
        public object? Oldest { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            [JsonPropertyName("channel")]
            public required string Channel { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
            [JsonPropertyName("include_all_metadata")]
            public bool? IncludeAllMetadata { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("cursor")]
            public string Cursor { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("oldest")]
            public string Oldest { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("inclusive")]
            public bool? Inclusive { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("limit")]
            public int Limit { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("latest")]
            public string Latest { get; set; }

        }

    }
}
