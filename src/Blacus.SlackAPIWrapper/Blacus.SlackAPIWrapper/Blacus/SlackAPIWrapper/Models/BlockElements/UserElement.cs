using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class UserElement : BlockElement
    {

        public UserElement(string actionId) : base(actionId)
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public string? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user_id")]
        public string? UserId { get; set; }

    }

}