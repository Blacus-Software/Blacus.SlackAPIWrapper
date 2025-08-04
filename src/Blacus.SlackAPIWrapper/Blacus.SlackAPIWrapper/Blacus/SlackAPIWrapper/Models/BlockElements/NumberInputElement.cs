using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class NumberInputElement : BlockElement
    {

        public NumberInputElement(string actionId) : base(actionId)
        {
        }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("is_decimal_allowed")]
        public bool? IsDecimalAllowed { get; set; }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_value")]
        public string? InitialValue { get; set; }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("min_value")]
        public string? MinValue { get; set; }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("max_value")]
        public string? MaxValue { get; set; }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("dispatch_action_config")]
        public object? DispatchActionConfig { get; set; }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("focus_on_load")]
        public bool? FocusOnLoad { get; set; }

        [JsonPropertyOrder(6)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("placeholder")]
        public object? Placeholder { get; set; }

    }

}