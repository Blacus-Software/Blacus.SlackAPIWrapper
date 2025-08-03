using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper
{

    public class SlackCredential
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AppId { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClientId { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClientSecret { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string SigningSecret { set; get; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string VerificationToken { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string AppToken { get; set; } = string.Empty;

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string BotUserToken { get; set; } = string.Empty;

        public string ToJson() => JsonSerializer.Serialize<SlackCredential>(this, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        public static SlackCredential? FromJson(string json)
        {

            if (string.IsNullOrWhiteSpace(json))
                return default;

            return JsonSerializer.Deserialize<SlackCredential>(json);

        }

    }

}
