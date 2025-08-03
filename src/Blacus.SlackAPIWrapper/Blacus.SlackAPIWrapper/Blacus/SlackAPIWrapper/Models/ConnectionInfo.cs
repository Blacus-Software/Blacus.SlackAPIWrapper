using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public partial class ConnectionInfo
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("app_id")]
        public string AppId { get; set; }

    }

}