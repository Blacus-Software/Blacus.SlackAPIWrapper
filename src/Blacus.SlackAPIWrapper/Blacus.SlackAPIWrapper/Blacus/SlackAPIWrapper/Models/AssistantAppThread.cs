using Blacus.SlackAPIWrapper.Models.Blocks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class AssistantAppThread : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("title_blocks")]
        public List<SlackBlock>? TitleBlocks { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("artifacts")]
        public List<object>? Artifacts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("context")]
        public AssistantContext? Context { get; set; }

    }

}