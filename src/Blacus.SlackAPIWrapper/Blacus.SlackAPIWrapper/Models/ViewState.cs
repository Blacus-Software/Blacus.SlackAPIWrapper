using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class ViewState : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("values")]
        public Dictionary<string, Dictionary<string, ViewStateValuesOptions>>? Values { get; set; }

    }

}