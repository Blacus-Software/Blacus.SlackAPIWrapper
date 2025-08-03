using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public partial class SlackWebSocketDebugInfo
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("host")]
        public string Host { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("build_number")]
        public long? BuildNumber { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("approximate_connection_time")]
        public long? ApproximateConnectionTime { get; set; }

    }

}