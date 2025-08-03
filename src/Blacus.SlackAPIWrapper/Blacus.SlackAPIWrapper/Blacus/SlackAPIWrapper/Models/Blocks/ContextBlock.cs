using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{
    public class ContextBlock : SlackBlock
    {

        public ContextBlock(string blockId) : base(blockId)
        {
            //Type = "context";
        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("elements")]
        public List<object> Elements { get; set; }

    }
}
