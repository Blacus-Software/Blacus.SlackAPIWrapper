using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    public class SlackWebSocketTransmissionEventsApi : ISlackWebSocketPayload
    {

        public string? SourceJson { get; set; }
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketClient? Client { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketAcknowledge? AcknowledgePayload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("payload")]
        public ISlackPayload? Payload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("envelope_id")]
        public Guid? EnvelopeId { get; set; }

    }

}