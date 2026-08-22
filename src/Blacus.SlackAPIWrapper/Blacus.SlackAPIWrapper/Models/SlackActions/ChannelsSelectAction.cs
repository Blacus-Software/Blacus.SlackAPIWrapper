using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    public class ChannelsSelectAction : SlackAction
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_channel")]
        public string? SelectedChannel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_channel")]
        public string? InitialChannel { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("response_url_enabled")]
        public bool? ResponseUrlEnabled { get; set; }

    }

}
