using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("users.list", "Get", SlackMethodContentType.Json)]
    public class UsersList : SlackMethod
    {

        public static async Task<UsersList> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<UsersList>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("members")]
        public List<User>? Members { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("cache_ts")]
        public long? CacheTs { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("offset")]
        public string? Offset { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("team_id")]
            public string? TeamId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("cursor")]
            public string? Cursor { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("limit")]
            public long? Limit { get; set; }

        }

    }

}
