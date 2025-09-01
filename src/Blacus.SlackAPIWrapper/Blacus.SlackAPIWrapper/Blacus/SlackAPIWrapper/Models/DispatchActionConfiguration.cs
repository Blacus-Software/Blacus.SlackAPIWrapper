using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class DispatchActionConfiguration : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("trigger_actions_on")]
        public string[]? TriggerActionsOn { get; set; }

    }

}