using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Methods;
using Blacus.SlackAPIWrapper.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper
{

    [SlackMethod("users.conversations", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class UsersConversations : SlackMethod
    {

        public static async Task<UsersConversations> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<UsersConversations>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channels")]
        public List<Channel>? Channels { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("cursor")]
            public string? Cursor { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("exclude_archived")]
            public bool? ExcludeArchived { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("limit")]
            public int? Limit { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("team_id")]
            public string? TeamId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("types")]
            public string? Types { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("user")]
            public string? User { get; set; }

        }

    }

}