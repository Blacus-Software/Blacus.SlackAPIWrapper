using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionHello), typeDiscriminator: "hello")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionSlashCommand), typeDiscriminator: "slash_commands")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionInteractive), typeDiscriminator: "interactive")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionDisconnect), typeDiscriminator: "disconnect")]
    [JsonDerivedType(typeof(SlackWebSocketTransmissionEventsApi), typeDiscriminator: "events_api")] 
    public interface ISlackWebSocketBase : ISlackModel
    {

        public SlackWebSocketClient? Client { get; set; }

        public SlackWebSocketAcknowledge? AcknowledgePayload { get; set; }

        public Guid? EnvelopeId { get; set; }

        public ISlackPayload? GetPayload()
        {
            if (this is ISlackWebSocketPayload payloadContainer)
                return payloadContainer.Payload;

            return null;
        }

    }
    
    public interface ISlackWebSocketPayload : ISlackWebSocketBase
    {

        public ISlackPayload? Payload { get; set; }

    }

}
