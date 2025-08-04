using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{
    public class RichTextSectionElement : BlockElement
    {

        public RichTextSectionElement(string actionId) : base(actionId)
        {

        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("elements")]
        public BlockElementCollection? Elements { get; set; }

    }
}