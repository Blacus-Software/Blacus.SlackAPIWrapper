using Blacus.SlackAPIWrapper.JsonConverters;
using System.Collections.Generic;
using System.Text.Json;

namespace Blacus.SlackAPIWrapper.Models
{

    public interface ISlackModel
    {

        public string? SourceJson { get; set; }
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        public static T? FromJson<T>(string json) where T : ISlackModel
        {
            ISlackModel toReturn = JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton);
            toReturn.SourceJson = json;
            return (T?)toReturn;
        }

        public static T? FromJson<T>(System.IO.Stream json) where T : ISlackModel
        {
            ISlackModel toReturn = JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton); 
            return (T?)toReturn;
        }

    }

}