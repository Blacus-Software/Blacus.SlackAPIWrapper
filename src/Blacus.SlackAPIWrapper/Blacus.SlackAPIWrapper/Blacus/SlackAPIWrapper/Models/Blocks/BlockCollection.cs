using System.Collections.Generic;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class BlockCollection : List<SlackBlock>
    {

        public string ToJson() =>
            System.Text.Json.JsonSerializer.Serialize<object>(this, JsonConverters.JsonConverterSettings.Singleton);

    }

}