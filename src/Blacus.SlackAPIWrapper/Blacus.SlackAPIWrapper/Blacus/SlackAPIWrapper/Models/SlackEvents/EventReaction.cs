using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackEvents
{

    public class EventReaction : SlackEvent
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("reaction")]
        public string? Reaction { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("item")]
        public Item? Item { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("item_user")]
        public string? ItemUser { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("event_ts")]
        public string? EventTs { get; set; }

    }

}
