using System.Text.Json.Serialization;

#pragma warning disable CS8603, CS8600, CS8602

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{
    public class BlockOptionObject
    {

        private const int field_MaxTextLength = 75;

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("text")]
        public TextObject Text
        {
            get { return field; }
            set
            {
                field = value;
                field.MaxTextLength = field_MaxTextLength;
            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("description")]
        public TextObject? Description { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }

    }

}