using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class Item : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? Type { get; set; }
        // Example Value: message

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("Channel")]
        public string? Channel { get; set; }
        // Example Value: C04PT7JEAV9

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("ts")]
        public string? Ts { get; set; }
        // Example Value: 1754170820.384759

    }

}
