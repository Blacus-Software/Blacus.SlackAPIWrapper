using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class BoolConverter : JsonConverter<bool>
    {

        public override bool CanConvert(Type t) => t == typeof(bool);

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                    return true;

                case JsonTokenType.False:
                    return false;

                case JsonTokenType.String:
                    var stringValue = reader.GetString();

                    if (bool.TryParse(stringValue, out var parsedBool))
                        return parsedBool;

                    if (stringValue == "1")
                        return true;

                    if (stringValue == "0")
                        return false;

                    break;

                case JsonTokenType.Number:
                    if (reader.TryGetInt64(out var numValue))
                        return numValue != 0;

                    break;

            }

            throw new JsonException($"Cannot convert token '{reader.TokenType}' to bool.");

        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) =>
            writer.WriteBooleanValue(value);

        public static readonly BoolConverter Singleton = new BoolConverter();

    }

}
