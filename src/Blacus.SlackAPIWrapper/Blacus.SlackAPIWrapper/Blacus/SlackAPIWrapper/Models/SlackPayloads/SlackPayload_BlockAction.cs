using Blacus.SlackAPIWrapper.Models.SlackActions;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public class SlackPayload_BlockAction : SlackPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("actions")] 
        public ISlackAction[]? Actions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_url")]
        public Uri? ResponseUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_urls")]
        public ResponseUrls[]? ResponseUrls { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("trigger_id")]
        public string? TriggerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public User? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("container")]
        public Container? Container { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel")]
        public Channel? Channel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("message")]
        public Message? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("state")]
        public State? State { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("view")]
        public View? View { get; set; }

    }

}
