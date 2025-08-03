using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{
    public class SlackEvent : SlackModel, ISlackPayload_EventCallback_Event
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

    }
}