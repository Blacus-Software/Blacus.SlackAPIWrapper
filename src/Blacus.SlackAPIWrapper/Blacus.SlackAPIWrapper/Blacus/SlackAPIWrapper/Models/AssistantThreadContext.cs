using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class AssistantThreadContext : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_id")]
        public string ChannelId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team_id")]
        public string TeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise_id")]
        public string EnterpriseId { get; set; }

    }

}