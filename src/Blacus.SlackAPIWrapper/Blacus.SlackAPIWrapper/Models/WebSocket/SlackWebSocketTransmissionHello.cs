using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    public class SlackWebSocketTransmissionHello : SlackWebSocketTransmission
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("connection_info")]
        public ConnectionInfo? ConnectionInfo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("num_connections")]
        public long? NumConnections { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("debug_info")]
        public SlackWebSocketDebugInfo? DebugInfo { get; set; }

    }

}
