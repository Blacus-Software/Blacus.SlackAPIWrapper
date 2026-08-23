using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.WebSocket
{

    public class SlackWebSocketTransmissionSlashCommand : SlackWebSocketTransmission
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("payload")]
        public new SlackSlashCommandPayload? Payload
        {
            get => base.Payload as SlackSlashCommandPayload;
            set => base.Payload = value;
        }

    }

}
