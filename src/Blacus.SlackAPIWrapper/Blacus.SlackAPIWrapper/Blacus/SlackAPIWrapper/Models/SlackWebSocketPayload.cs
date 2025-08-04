using Blacus.SlackAPIWrapper.JsonConverters;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public interface ISlackWebSocketPayload : ISlackModel
    {

        public string? TypeString { get; set; }
        public SlackWebSocketPayloadType? Type { get; }

    }

    public enum SlackWebSocketPayloadType
    {
        SlashCommands,
        Interactive,
        Hello
    }

    public class SlackWebSocketPayload : SlackModel, ISlackWebSocketPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public SlackWebSocketPayloadType? Type
        {
            get => Enum.Parse<SlackWebSocketPayloadType>(TypeString.Replace("_", string.Empty), true);
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? TypeString { get; set; }

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