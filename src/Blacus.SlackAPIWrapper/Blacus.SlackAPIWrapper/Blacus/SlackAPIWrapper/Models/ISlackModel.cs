using Blacus.SlackAPIWrapper.JsonConverters;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace Blacus.SlackAPIWrapper.Models
{

    public interface ISlackModel
    {

        public string? SourceJson { get; set; }
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        public static ISlackModel? FromJson(string json) => FromJson<SlackModel>(json);

        public static T? FromJson<T>(string json, JsonSerializerOptions? options = null) where T : ISlackModel
        {

            if (string.IsNullOrWhiteSpace(json))
                throw new NullReferenceException();

            options ??= JsonConverterSettings.Singleton;

            ISlackModel toReturn = JsonSerializer.Deserialize<T>(json, options);
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