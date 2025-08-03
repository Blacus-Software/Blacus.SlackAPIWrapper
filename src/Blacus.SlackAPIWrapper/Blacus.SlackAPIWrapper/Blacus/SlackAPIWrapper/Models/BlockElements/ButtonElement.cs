using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements;

public class ButtonElement : BlockElement
{

    public ButtonElement(string actionId = null) : base(actionId)
    {
        Text = new TextObject();
    }

    [JsonPropertyOrder(2)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("text")]
    public TextObject? Text { get; set; }

    [JsonPropertyOrder(6)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("url")]
    public string? Url { get; set; }

    [JsonPropertyOrder(0)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("style")]
    public ButtonStyle? Style { get; set; }

    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("confirm")]
    public object? Confirm { get; set; }

    [JsonPropertyOrder(5)]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("accessibility_label")]
    public string? AccessibilityLabel { get; set; }

    public enum ButtonStyle
    {
        primary,
        danger,
    }

}