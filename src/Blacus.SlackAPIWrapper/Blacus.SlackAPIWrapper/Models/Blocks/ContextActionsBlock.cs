using Blacus.SlackAPIWrapper.Models.Blocks;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class ContextActionsBlock : SlackBlock
    {

        public ContextActionsBlock(string? blockId = null) : base(blockId)
        {
            
            if (blockId is null)
                blockId = Guid.NewGuid().ToString();

        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("elements")]
        public List<object>? Elements { get; set; }

    }

}
