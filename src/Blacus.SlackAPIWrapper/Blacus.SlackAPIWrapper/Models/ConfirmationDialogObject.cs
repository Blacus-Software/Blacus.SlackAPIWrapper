using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class ConfirmationDialogObject : SlackModel
    {

        private const int field_MaxTextLength = 300;

        public ConfirmationDialogObject()
        {

            Title = new TextObject() { Type = TextObject.TextType.PlainText, };
            Text = new TextObject() { Text = string.Empty, };

        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("title")]
        public TextObject? Title { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public TextObject? Text
        {
            get { return field; }
            set
            {
                field = value;
                if (field != null)
                {
                    field.MaxTextLength = field_MaxTextLength;
                }
            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("confirm")]
        public TextObject? Confirm { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("deny")]
        public TextObject? Deny { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("style")]
        public string? Style { get; set; }

    }

}