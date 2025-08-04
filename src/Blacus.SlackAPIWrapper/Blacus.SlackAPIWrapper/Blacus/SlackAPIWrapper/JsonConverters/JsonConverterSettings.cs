using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class JsonConverterSettings
    {
        public static readonly System.Text.Json.JsonSerializerOptions Singleton = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                DecodingChoiceConverter.Singleton,
            }
        };
    }

}
