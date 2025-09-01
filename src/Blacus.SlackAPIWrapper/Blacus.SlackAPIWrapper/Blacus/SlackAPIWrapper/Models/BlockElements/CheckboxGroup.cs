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
        [JsonPropertyName("options")]
        public List<SlackModel> Options { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("initial_options")]
        public List<SlackModel>? InitialOptions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("confirm")]
        public ConfirmationDialogObject? Confirm { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("focus_on_load")]
        public bool? FocusOnLoad { get; set; }

    }

}