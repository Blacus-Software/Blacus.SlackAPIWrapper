using Blacus.SlackAPIWrapper.JsonConverters;
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

}