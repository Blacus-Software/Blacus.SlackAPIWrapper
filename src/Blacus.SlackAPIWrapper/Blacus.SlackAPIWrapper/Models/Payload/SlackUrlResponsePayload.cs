using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public class SlackUrlResponsePayload : SlackPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("challenge")]
        public string? Challenge { get; set; }

    }

}