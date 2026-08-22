using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {

        private const string format = "HH:mm";

        public override bool CanConvert(Type t) => t == typeof(TimeOnly);

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
            TimeOnly.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value.ToString(format), options);

    }

}
