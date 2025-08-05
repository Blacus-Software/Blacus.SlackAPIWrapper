using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public class SlackPayload_Command : SlackPayload
    {

        public SlackPayload_Command()
        { 
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("trigger_id")]
        public string? TriggerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_name")]
        public string? ChannelName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("command")]
        public string? Command { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_name")]
        public string? UserName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_url")]
        public Uri? ResponseUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team_id")]
        public string? TeamID { get; set; }
    }

}
