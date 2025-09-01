using Blacus.SlackAPIWrapper.JsonConverters;
using Blacus.SlackAPIWrapper.Models.SlackActions;
using Blacus.SlackAPIWrapper.Models.SlackEvents;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public class SlackPayload_FallBack : SlackPayload, ISlackPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public User? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_urls")]
        public ResponseUrls[]? ResponseUrls { get; set; }  

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("actions")]
        public ISlackAction[]? Actions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("event_id")]
        public string? EventId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("event_time")]
        public long? EventTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("is_ext_shared_channel")]
        public bool? IsExtSharedChannel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("event_context")]
        public string? EventContext { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("event")]
        [JsonConverter(typeof(EventConverter))]
        public ISlackPayloadEvent? Event { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("context_team_id")]
        public string? ContextTeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("context_enterprise_id")]
        public string? ContextEnterpriseId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SlackRequestContentType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SlackRequestBodyString { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SlackRequestSignature { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SlackRequestTimestamp { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SlackRequestJson { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("challenge")]
        public string? Challenge { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("authorizations")]
        public Authorization[]? Authorizations { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise_id")]
        public string? EnterpriseId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_id")]
        public string? ChannelId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("channel_name")]
        public string? ChannelName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_name")]
        public string? UserName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("command")]
        public string? Command { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("text")]
        public string? Text { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise_name")]
        public string? EnterpriseName { get; set; }

    }

}
