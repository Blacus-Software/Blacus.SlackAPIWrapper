using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{
    public class ImageBlock : SlackBlock
    {

        public enum ActionBlockType
        {

        }

        public ImageBlock(string blockId) : base(blockId)
        {
            //Type = "image";
            Elements = new();
        }

        [JsonPropertyOrder(3)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("elements")]
        public List<object> Elements { get; set; }

    }

}