using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SourceJson { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        public string ToJson() =>
            System.Text.Json.JsonSerializer.Serialize<object>(this, JsonConverterSettings.Singleton);

    }

    public class SlackModel<T> : SlackModel
    {

        public static T? FromJson(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton);
            }
            catch (System.Exception)
            {
                return default(T);
            }
        }

        public static T? FromJson(System.IO.Stream json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton);
            }
            catch (System.Exception)
            {
                return default(T);
            }
        }

    }

    public class JsonConverterSettings
    {
        public static readonly System.Text.Json.JsonSerializerOptions Singleton = new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                DecodingChoiceConverter.Singleton
            }
        };
    }

    internal class NullParseStringConverter : JsonConverter<bool?>
    {

        public override bool CanConvert(Type t) => t == typeof(bool?);

        public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            bool b;
            if (Boolean.TryParse(value, out b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
        {
            var boolString = value.Value ? "true" : "false";
            JsonSerializer.Serialize(writer, boolString, options);
            return;
        }

        public static readonly NullParseStringConverter Singleton = new();
    }

    internal class ParseStringConverter : JsonConverter<bool>
    {
        public override bool CanConvert(Type t) => t == typeof(bool);

        public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            bool b;
            if (Boolean.TryParse(value, out b))
            {
                return b;
            }
            throw new Exception("Cannot unmarshal type bool");
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            var boolString = value ? "true" : "false";
            JsonSerializer.Serialize(writer, boolString, options);
            return;
        }

        public static readonly ParseStringConverter Singleton = new();
    }

    internal class DecodingChoiceConverter : JsonConverter<bool>
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