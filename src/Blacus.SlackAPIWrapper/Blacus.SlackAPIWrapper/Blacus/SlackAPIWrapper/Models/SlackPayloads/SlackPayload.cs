using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Web;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{

    public interface ISlackPayload : ISlackModel
    {

        public PayloadType? Type { get; }

    }

    public enum PayloadType
    {
        BlockActions,
        ViewSubmission,
        ViewClosed,
        MessageActions,
        EventCallback,
        UrlVerification,
        SlashCommand
    }

    public class SlackPayload : SlackModel, ISlackPayload
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("token")]
        public string? Token { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public PayloadType? Type
        {
            get
            {

                if (string.IsNullOrWhiteSpace(TypeString))
                    return null;

                return Enum.Parse<PayloadType>(TypeString.Replace("_", string.Empty), true);

            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? TypeString { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("api_app_id")]
        public string? ApiAppId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team")]
        public Enterprise? Team { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise")]
        public Channel? Enterprise { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team_id")]
        public string? TeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team_domain")]
        public string? TeamDomain { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("is_enterprise_install")]
        public bool? IsEnterpriseInstall { get; set; }

    }

}