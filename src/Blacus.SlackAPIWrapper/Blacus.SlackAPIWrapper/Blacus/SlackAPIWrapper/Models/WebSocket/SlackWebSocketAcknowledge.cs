using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    public class SlackWebSocketAcknowledge : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("envelope_id")]
        public Guid? EnvelopeId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("payload")]
        public ISlackPayload? Payload { get; set; }

    }

}