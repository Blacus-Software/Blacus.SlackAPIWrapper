using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class DateTimeOffsetAsIntConverter : JsonConverter<DateTimeOffset>
    {

        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Number)
            {
                double value = reader.GetDouble();
                return DateTimeOffset.FromUnixTimeSeconds((long)value);
            }

            if (reader.TokenType == JsonTokenType.String)
            {
                var str = reader.GetString();
                if (double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue))
                    return DateTimeOffset.FromUnixTimeSeconds((long)doubleValue);

                if (DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dto))
                    return dto;
            }

            throw new JsonException($"Cannot convert token '{reader.TokenType}' to DateTimeOffset.");
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
            writer.WriteNumberValue(value.ToUnixTimeSeconds());

    }

}
