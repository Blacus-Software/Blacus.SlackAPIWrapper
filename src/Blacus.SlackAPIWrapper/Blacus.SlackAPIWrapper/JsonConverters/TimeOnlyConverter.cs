using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {

        private const string format = "HH:mm";

        public override bool CanConvert(Type t) => t == typeof(TimeOnly);

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str != null && TimeOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var time))
                return time;

            if (str != null && TimeOnly.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out time))
                return time;

            throw new JsonException($"Cannot parse '{str}' as TimeOnly");
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString(format, CultureInfo.InvariantCulture));

    }

}
