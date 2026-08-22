using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class Authorization : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise_id")]
        public string? EnterpriseId { get; set; }

        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("is_bot")]
        public bool? IsBot { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("is_enterprise_install")]
        public bool? IsEnterpriseInstall { get; set; }

    }

}