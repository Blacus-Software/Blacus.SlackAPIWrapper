using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class SlackEvent_ReactionRemoved : SlackEvent_Reaction, ISlackPayload_EventCallback_Event
    {
    }

    public class SlackEvent_ReactionAdded : SlackEvent_Reaction
    {
    }

    public class SlackEvent_Reaction : SlackEvent
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
