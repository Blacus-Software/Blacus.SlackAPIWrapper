using Blacus.SlackAPIWrapper.Models.BlockElements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class SectionBlock : SlackBlock
    {

        public SectionBlock(string? blockId = null) : base(blockId)
        { }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public TextObject? Text { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("fields")]
        public List<object>? Fields { get; set; }


        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("accessory")]
        public BlockElement? Accessory { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("expand")]
        public bool? Expand { get; set; }

    }

}