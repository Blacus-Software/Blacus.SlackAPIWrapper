using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class InputBlock : SlackBlock
    {

        public InputBlock(string? blockId = null) : base(blockId)
        {
        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("label")]
        public TextObject? Label { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("element")]
        public BlockElements.BlockElement? Element { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dispatch_action")]
        public bool? DispatchAction { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("hint")]
        public TextObject? Hint { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("optional")]
        public bool? Optional { get; set; }

    }

}
