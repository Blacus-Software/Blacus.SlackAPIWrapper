using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionHello), typeDiscriminator: "hello")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionSlashCommand), typeDiscriminator: "slash_commands")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionInteractive), typeDiscriminator: "interactive")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionDisconnect), typeDiscriminator: "disconnect")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionEventsApi), typeDiscriminator: "events_api")]
    public abstract class SlackWebSocketTransmission : SlackModel, ISlackWebSocketPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketClient? Client { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketAcknowledge? AcknowledgePayload { get; set; } 

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("envelope_id")]
        public Guid? EnvelopeId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("payload")]
        public virtual ISlackPayload? Payload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("retry_attempt")]
        public long? RetryAttempt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("accepts_response_payload")]
        public bool? AcceptsResponsePayload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("retry_reason")]
        public string? RetryReason { get; set; }

        public static SlackWebSocketTransmission? FromJson(string json)
        {

            if (string.IsNullOrWhiteSpace(json))
                throw new ArgumentException("JSON cannot be null or whitespace.", nameof(json));

            var toReturn = JsonSerializer.Deserialize<SlackWebSocketTransmission>(json, JsonConverters.JsonConverterSettings.Singleton);
            if (toReturn != null)
                toReturn.SourceJson = json;

            return toReturn;

        }

    }

}