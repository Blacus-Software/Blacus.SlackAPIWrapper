using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{
    public class TextElement : BlockElement
    {

        public TextElement(string actionId) : base(actionId) { }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public string? Text { get; set; }



    }
}