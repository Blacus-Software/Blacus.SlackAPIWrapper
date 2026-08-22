using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{
     
    public class DateOnlyConverter : JsonConverter<DateOnly>
    {

        private const string format = "yyyy-MM-dd";

        public override bool CanConvert(Type t) => t == typeof(TimeOnly);

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            DateOnly.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value.ToString(format), options);

    }

}
