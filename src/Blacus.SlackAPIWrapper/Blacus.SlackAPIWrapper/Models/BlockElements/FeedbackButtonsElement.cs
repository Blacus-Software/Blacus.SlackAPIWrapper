using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class FeedbackButtonsElement : BlockElement
    {

        public FeedbackButtonsElement(string? actionId = null) : base(actionId)
        {
        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("positive_button")]
        public ButtonElement PositiveButton { get; set; } 

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("negative_button")]
        public ButtonElement NegativeButton { get; set; } 

    }

}
