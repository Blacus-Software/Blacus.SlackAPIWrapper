using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public static class JsonConverterSettings
    {

        public static readonly JsonSerializerOptions Singleton = Generate();

        public static JsonSerializerOptions Generate()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                Converters =
                {
                    BoolConverter.Singleton,
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    new DateTimeOffsetAsIntConverter(),
                    new TimeOnlyConverter(),
                    new DateOnlyConverter()
                }
            };
#if NET9_0_OR_GREATER
            options.AllowOutOfOrderMetadataProperties = true;
#endif
            return options;
        }

    }

}
