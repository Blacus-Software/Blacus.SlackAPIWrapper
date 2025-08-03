using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{
    public class AssistantThread : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_id")]
        public string UserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("context")]
        public AssistantThreadContext Context { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("thread_ts")]
        public string ThreadTs { get; set; }

    }
}