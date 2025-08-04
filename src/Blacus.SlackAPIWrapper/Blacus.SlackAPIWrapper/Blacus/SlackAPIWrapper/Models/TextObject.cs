using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class TextObject : SlackModel
    {

        public TextObject() { }

        public TextObject(string text, TextType textType = TextType.Mrkdwn, bool emoji = true)
        {
            Text = text;
            Type = textType;
            Emoji = emoji;
        }

        public enum TextType
        {
            PlainText,
            Mrkdwn
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public TextType Type { get; set; } = TextType.PlainText;

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public string? TypeString
        {
            get
            {
                switch (Type)
                {
                    default:
                    case TextType.PlainText:
                        return "plain_text";

                    case TextType.Mrkdwn:
                        return "mrkdwn";
                }
            }
            set
            {
                switch (value)
                {
                    default:
                    case "plain_text":
                        Type = TextType.PlainText;
                        break;

                    case "mrkdwn":
                        Type = TextType.Mrkdwn;
                        break;
                }
            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public string? Text
        {
            get
            {

                if (MaxTextLength != null)
                {
                    return text.Substring(0, Math.Min(MaxTextLength ?? 3000, text.Length));
                }

                return text;

            }
            set { text = value; }
        }
        private string? text;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("emoji")]
        public bool? Emoji
        {
            get
            {
                if (Type != TextType.PlainText) { return null; }
                return _emoji;
            }
            set { _emoji = value; }
        }
        private bool? _emoji;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("verbatim")]
        public bool? Verbatim { get; set; }

        [JsonIgnore]
        public int? MaxTextLength { get; set; }

    }

}