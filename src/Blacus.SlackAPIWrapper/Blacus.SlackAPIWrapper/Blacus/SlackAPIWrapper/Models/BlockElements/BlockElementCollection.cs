using System.Collections.Generic;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class BlockElementCollection : List<BlockElement>
    {

        public string ToJson() =>
            System.Text.Json.JsonSerializer.Serialize<object>(this, JsonConverterSettings.Singleton);

    }

}