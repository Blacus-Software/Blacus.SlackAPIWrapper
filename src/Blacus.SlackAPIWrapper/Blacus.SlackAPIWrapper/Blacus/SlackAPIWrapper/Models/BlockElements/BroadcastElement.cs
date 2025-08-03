using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class BroadcastElement : BlockElement
    {

        public BroadcastElement(string actionId) : base(actionId)
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("range")]
        public string? Range { get; set; }

    }

}