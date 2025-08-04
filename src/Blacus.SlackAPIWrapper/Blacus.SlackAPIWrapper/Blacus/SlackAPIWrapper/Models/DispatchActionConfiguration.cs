using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class DispatchActionConfiguration : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("trigger_actions_on")]
        public List<string>? TriggerActionsOn { get; set; }

    }

}