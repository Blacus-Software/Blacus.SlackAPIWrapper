using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackEvents
{

    public class SlackEvent : SlackModel, ISlackPayloadEvent
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

    }

}