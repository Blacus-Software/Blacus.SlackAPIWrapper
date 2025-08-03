using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{
    public class BlockOptionObject
    {

        private const int field_Text_MaxLength = 75;

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("text")]
        public TextObject Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _text.MaxTextLength = field_Text_MaxLength;
            }
        }
        private TextObject _text;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("description")]
        public TextObject Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("value")]
        public string Value { get; set; }

    }

}