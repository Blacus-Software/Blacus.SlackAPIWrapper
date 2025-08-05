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
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                BoolConverter.Singleton,
                SlackActionConverter.Singleton,
                SlackPayloadConverter.Singleton
            }
        };

    }

}
