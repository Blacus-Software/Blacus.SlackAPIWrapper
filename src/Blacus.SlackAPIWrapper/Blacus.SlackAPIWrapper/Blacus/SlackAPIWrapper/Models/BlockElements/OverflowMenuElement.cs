using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class OverflowMenuElement : BlockElement
    {

        public OverflowMenuElement(string actionId) : base(actionId)
        {
        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("options")]
        public List<object>? Options { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("confirm")]
        public ConfirmationDialogObject? Confirm { get; set; }

    }

}