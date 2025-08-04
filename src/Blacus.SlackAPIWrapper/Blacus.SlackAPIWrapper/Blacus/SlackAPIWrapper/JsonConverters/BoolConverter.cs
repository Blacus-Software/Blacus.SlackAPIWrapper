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

            bool boolValue;

            switch (reader.TokenType)
            {
                case JsonTokenType.True:
                case JsonTokenType.False:
                    boolValue = reader.GetBoolean();
                    return boolValue;

                case JsonTokenType.String:
                    var stringValue = reader.GetString(); 
                    
                    if (Boolean.TryParse(stringValue, out boolValue))
                        return boolValue;

                    break;

            }

            throw new Exception("Cannot unmarshal type bool");

        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)=>
            JsonSerializer.Serialize(writer, value);  

        public static readonly BoolConverter Singleton = new BoolConverter();

    }

}
