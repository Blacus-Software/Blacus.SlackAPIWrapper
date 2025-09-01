using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    [SlackAction(ActionType.Checkboxes)]
    public class CheckBoxAction : SlackAction
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_options")]
        public OptionObject[]? SelectedOptions { get; set; }

    }

}