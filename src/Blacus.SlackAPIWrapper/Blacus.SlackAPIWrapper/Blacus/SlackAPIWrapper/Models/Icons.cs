using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{
    public class Icons : SlackModel

    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("image_36")]
        public Uri? Image36 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("image_48")]
        public Uri? Image48 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("image_72")]
        public Uri? Image72 { get; set; }
    }
}