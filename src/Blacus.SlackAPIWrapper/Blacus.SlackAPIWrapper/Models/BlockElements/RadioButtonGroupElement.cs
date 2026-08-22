using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class RadioButtonGroupElement : BlockElement
    {

        public RadioButtonGroupElement(string actionId) : base(actionId)
        {
        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("options")]
        public List<CheckBoxOption>? Options { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_options")]
        public List<object>? InitialOptions { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("confirm")]
        public ConfirmationDialogObject? Confirm { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("focus_on_load")]
        public bool? FocusOnLoad { get; set; }

    }

}