using Blacus.SlackAPIWrapper.Models.BlockElements;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class RichTextSectionBlock : SlackBlock
    {

        public RichTextSectionBlock(string? blockId) : base(blockId)
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("elements")]
        public BlockElementCollection? Elements { get; set; }

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("element")]
        public BlockElement? Element { get; set; }

    }

}