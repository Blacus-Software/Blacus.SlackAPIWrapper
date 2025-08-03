using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class TextObject : SlackModel
    {


        public TextObject() { }

        public TextObject(string text)
        {
            Text = text;
        }

        public enum TextType
        {
            plain_text,
            mrkdwn
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public TextType Type { get; set; } = TextType.plain_text;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public string Text
        {
            get
            {
                if (text == null && ThrowExceptionWhenNull)
                {
                    throw new ArgumentNullException(nameof(Text));
                }

                if (MaxTextLength != null)
                {
                    return text.Substring(0, Math.Min(MaxTextLength ?? 9999, text.Length));
                }

                return text;
            }
            set { text = value; }
        }
        private string text;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("emoji")]
        public bool? Emoji
        {
            get
            {
                if (Type != TextType.plain_text) { return null; }
                return _emoji;
            }
            set { _emoji = value; }
        }
        private bool? _emoji;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("verbatim")]
        public bool? Verbatim { get; set; }

        [JsonIgnore]
        public bool ThrowExceptionWhenNull { get; set; } = false;

        [JsonIgnore]
        public int? MaxTextLength { get; set; }

    }

}