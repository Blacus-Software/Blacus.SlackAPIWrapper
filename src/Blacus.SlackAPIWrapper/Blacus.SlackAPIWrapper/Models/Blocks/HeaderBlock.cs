using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class HeaderBlock : SlackBlock
    {

        public HeaderBlock(string? blockId = null) : base(blockId) { }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public TextObject? Text { get; set; }

    }

}
