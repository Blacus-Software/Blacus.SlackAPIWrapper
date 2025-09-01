using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public class SlackPayload_ResponseAction : ISlackPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_action")]
        public string? ResponseAction { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("errors")]
        public Dictionary<string, string>? Errors { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public PayloadType? Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Message? Message { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public View? View { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? TriggerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? ApiAppId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SourceJson { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

    }

}
