using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{
    public class ResponseMetaData : SlackModel
    {

        [JsonPropertyName("next_cursor")]
        public string? NextCursor { get; set; }

        [JsonPropertyName("messages")]
        public string[]? Messages { get; set; }

    }
}