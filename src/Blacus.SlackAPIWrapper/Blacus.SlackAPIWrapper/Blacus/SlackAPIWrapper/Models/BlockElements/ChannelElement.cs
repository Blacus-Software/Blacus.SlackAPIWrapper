using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class ChannelElement : BlockElement
    {

        public ChannelElement(string actionId) : base(actionId)
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("Channel")]
        public string? Channel { get; set; }

    }

}