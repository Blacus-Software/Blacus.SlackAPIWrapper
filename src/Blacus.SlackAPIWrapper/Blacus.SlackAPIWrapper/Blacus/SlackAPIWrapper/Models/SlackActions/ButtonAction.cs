using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    [SlackAction(ActionType.Button)]
    public class ButtonAction : SlackAction
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public TextObject? Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }

    }

    public class SlackActionAttribute : Attribute
    {

        public ActionType ActionType { get; }

        public SlackActionAttribute(ActionType actionType)
        {
            ActionType = actionType;
        }

    }

}
