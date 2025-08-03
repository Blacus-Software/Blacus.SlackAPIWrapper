using Blacus.SlackAPIWrapper.Models.BlockElements;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class ActionBlock : SlackBlock
    {

        public ActionBlock(string? blockId = null) : base(blockId)
        {
            Elements = new();
        }

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("elements")]
        public BlockElementCollection Elements { get; set; }

    }
}