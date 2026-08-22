using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackPayloads
{


    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(SlackViewSubmissionPayload), typeDiscriminator: "view_submission")]
    [JsonDerivedType(typeof(SlackSlashCommandPayload), typeDiscriminator: "slash_commands")]
    [JsonDerivedType(typeof(SlackBlockActionPayload), typeDiscriminator: "block_actions")]
    [JsonDerivedType(typeof(SlackViewClosedPayload), typeDiscriminator: "view_closed")]
    [JsonDerivedType(typeof(SlackUrlResponsePayload), typeDiscriminator: "url_verification")]
    [JsonDerivedType(typeof(SlackUrlEventCallbackPayload), typeDiscriminator: "event_callback")]

    //    MessageActions,

    public interface ISlackPayload : ISlackModel
    {

        public Message? Message { get; set; }

        public View? View { get; set; }

        public string? TriggerId { get; set; }

        public string? ApiAppId { get; set; }

    }

}