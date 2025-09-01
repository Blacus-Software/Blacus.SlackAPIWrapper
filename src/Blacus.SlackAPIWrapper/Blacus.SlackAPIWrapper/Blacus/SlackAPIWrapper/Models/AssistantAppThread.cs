using Blacus.SlackAPIWrapper.Models.Blocks;
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
        public SlackBlock[]? TitleBlocks { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("artifacts")]
        public SlackModel[]? Artifacts { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("context")]
        public AssistantContext? Context { get; set; }

    }

}