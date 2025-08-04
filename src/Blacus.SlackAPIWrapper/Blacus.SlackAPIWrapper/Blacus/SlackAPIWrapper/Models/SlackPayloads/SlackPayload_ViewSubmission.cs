using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public class SlackPayload_ViewSubmission : SlackPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public User? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_urls")]
        public ResponseUrls[]? ResponseUrls { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("trigger_id")]
        public string? TriggerId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("view")]
        public View? View { get; set; }

    }

}