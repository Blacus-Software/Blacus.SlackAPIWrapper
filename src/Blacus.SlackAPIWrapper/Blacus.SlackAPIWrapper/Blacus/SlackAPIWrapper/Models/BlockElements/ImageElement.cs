using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class ImageElement : BlockElement
    {

        public ImageElement() : base(null)
        {

        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("type")]
        public new string Type { get => "image"; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("image_url")]
        public string ImageUrl { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [JsonPropertyName("alt_text")]
        public string AltText { get; set; }

    }

}