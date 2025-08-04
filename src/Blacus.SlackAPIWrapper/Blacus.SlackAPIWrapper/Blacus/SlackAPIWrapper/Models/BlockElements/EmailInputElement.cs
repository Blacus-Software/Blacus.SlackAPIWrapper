using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class EmailInputElement : BlockElement
    {

        public EmailInputElement(string actionId) : base(actionId)
        {

        }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_value")]
        public TextObject? InitialValue { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dispatch_action_config")]
        public DispatchActionConfiguration? DispatchActionConfig { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("focus_on_load")]
        public DispatchActionConfiguration? FocusOnLoad { get; set; }

        [JsonPropertyOrder(2)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("placeholder")]
        public TextObject? Placeholder { get; set; }

    }

}