using Blacus.SlackAPIWrapper.Models.BlockElements;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public partial class State : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("values")]
        public Dictionary<string, Dictionary<string, BlockElement>> Values { get; set; }

    }

}
