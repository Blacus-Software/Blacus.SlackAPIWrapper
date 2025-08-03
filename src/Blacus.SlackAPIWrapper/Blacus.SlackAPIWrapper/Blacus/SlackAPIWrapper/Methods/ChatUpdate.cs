using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("chat.update", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ChatUpdate : SlackMethod
    {

        public static async Task<ChatUpdate> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.MakeHttpRequestAsync<ChatUpdate>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel")]
        public string Channel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ts")]
        public string Ts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            public string message { get; set; }
            public string channel { get; set; }
            public bool? as_user { get; set; }
            public string? blocks { get; set; }
            public string? file_ids { get; set; }
            public string? link_names { get; set; }
            public string? metadata { get; set; }
            public string? parse { get; set; }
            public bool? reply_broadcast { get; set; }
            public string? text { get; set; }
            public string? ts { get; set; }
            public string? username { get; set; }
            public string? attachments { get; set; }

        }

    }

}
