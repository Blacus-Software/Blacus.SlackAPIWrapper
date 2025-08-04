using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class DecodingChoiceConverter : JsonConverter<bool>
    {

        public override bool CanConvert(Type t) => t == typeof(bool);

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                    var boolValue = reader.GetBoolean();
                    return boolValue;
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    bool b;
                    if (Boolean.TryParse(stringValue, out b))
                    {
                        return b;
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value);
            return;
        }

        public static readonly DecodingChoiceConverter Singleton = new DecodingChoiceConverter();

    }

}
