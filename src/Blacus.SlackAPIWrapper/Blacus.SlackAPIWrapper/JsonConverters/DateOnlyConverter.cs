using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{
     
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {

        private const string format = "yyyy-MM-dd";

        public override bool CanConvert(Type t) => t == typeof(DateOnly);

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var str = reader.GetString();
            if (str != null && DateOnly.TryParseExact(str, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                return date;

            if (str != null && DateOnly.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                return date;

            throw new JsonException($"Cannot parse '{str}' as DateOnly");
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
            writer.WriteStringValue(value.ToString(format, CultureInfo.InvariantCulture));

    }

}
