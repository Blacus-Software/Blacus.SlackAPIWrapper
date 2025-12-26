using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    public class SlackWebSocketTransmissionHello : ISlackWebSocketPayload
    {

        public string? SourceJson { get; set; }
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketClient Client { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketAcknowledge AcknowledgePayload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("connection_info")]
        public ConnectionInfo? ConnectionInfo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("num_connections")]
        public long? NumConnections { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("debug_info")]
        public SlackWebSocketDebugInfo? DebugInfo { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("envelope_id")]
        public Guid? EnvelopeId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("payload")]
        public ISlackPayload? Payload { get; set; }

    }

}
