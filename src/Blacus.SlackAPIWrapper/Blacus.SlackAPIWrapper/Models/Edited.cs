using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public partial class Edited : SlackModel
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ts")]
        public string? Ts { get; set; }
    }

}