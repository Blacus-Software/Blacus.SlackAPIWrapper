using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class LinkElement : BlockElement
    {

        public LinkElement(string actionId) : base(actionId)
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("link")]
        public Uri? Link { get; set; }


    }

}