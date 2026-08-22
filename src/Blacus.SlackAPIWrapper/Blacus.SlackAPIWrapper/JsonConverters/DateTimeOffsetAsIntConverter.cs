using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class DateTimeOffsetAsIntConverter : JsonConverter<DateTimeOffset>
    {

        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double value = reader.GetDouble();
            return DateTimeOffset.FromUnixTimeSeconds((long)value);
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value.ToUnixTimeSeconds(), options);

    }

}
