using System.Collections.Generic;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    public class BlockCollection : List<SlackBlock>
    {

        public string ToJson(bool writeIndented = false) =>
            System.Text.Json.JsonSerializer.Serialize<object>(this, JsonConverters.JsonConverterSettings.Generate(writeIndented));

    }

}