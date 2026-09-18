using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackEvents
{

    public class EventAppHomeOpened : SlackEvent, ISlackPayloadEvent
    {

        [JsonIgnore]
        public string Type { get; set; } = "app_home_opened";

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel")]
        public string? Channel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("tab")]
        public string? Tab { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("view")]
        public View? View { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("event_ts")]
        public string? EventTs { get; set; }

    }

}