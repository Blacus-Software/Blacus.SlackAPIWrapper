using Blacus.SlackAPIWrapper.JsonConverters;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS8603, CS8600, CS8602, CS1998, CS8625, CS8618, CS8604

namespace Blacus.SlackAPIWrapper.Models
{

    public interface ISlackWebSocketPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SourceJson { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        public string? Type { get; set; }

    }

    public class SlackWebSocketPayload : SlackModel, ISlackWebSocketPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("envelope_id")]
        public Guid? EnvelopeId { get; set; }

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
        [JsonPropertyName("payload")]
        [JsonConverter(typeof(SlackPayloadConverter))]
        public ISlackPayload? Payload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("retry_attempt")]
        public long? RetryAttempt { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("accepts_response_payload")]
        public bool? AcceptsResponsePayload { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("retry_reason")]
        public string? RetryReason { get; set; }

        public static SlackWebSocketPayload FromJson(string json)
        {

            if (string.IsNullOrWhiteSpace(json))
                throw new NullReferenceException("Json is empty");

            SlackWebSocketPayload? toReturn = null;

            toReturn = JsonSerializer.Deserialize<SlackWebSocketPayload>(json, JsonConverters.JsonConverterSettings.Singleton);
            toReturn.SourceJson = json;

            return toReturn;

        }

    }

}