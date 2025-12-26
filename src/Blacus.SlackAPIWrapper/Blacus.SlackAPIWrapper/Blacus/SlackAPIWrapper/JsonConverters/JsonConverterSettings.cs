using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public static class JsonConverterSettings
    {

        public static readonly JsonSerializerOptions Singleton = Generate();

        public static JsonSerializerOptions Generate() => new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                BoolConverter.Singleton,
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                new DateTimeOffsetAsIntConverter(),
                new TimeOnlyConverter(),
                new DateOnlyConverter()
            },
            AllowOutOfOrderMetadataProperties = true,             
        };

    }

}
