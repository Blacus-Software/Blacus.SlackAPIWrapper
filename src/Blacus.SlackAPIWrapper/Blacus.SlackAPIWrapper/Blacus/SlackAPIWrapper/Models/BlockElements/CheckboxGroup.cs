using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class CheckboxGroup : BlockElement
    {

        public CheckboxGroup(string actionId) : base(actionId)
        {
            Options = new();
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public new string Type { get => "checkboxes"; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("options")]
        public List<object> Options { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("initial_options")]
        public List<object>? InitialOptions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("confirm")]
        public ConfirmationDialogObject? Confirm { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("focus_on_load")]
        public bool? FocusOnLoad { get; set; }

    }

}