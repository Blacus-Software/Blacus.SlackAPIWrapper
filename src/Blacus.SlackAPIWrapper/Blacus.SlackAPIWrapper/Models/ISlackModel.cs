using Blacus.SlackAPIWrapper.JsonConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

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
                throw new ArgumentException("JSON cannot be null or whitespace.", nameof(json));

            options ??= JsonConverterSettings.Singleton;

            T? toReturn = JsonSerializer.Deserialize<T>(json, options);
            if (toReturn != null)
                toReturn.SourceJson = json;

            return toReturn;

        }

        public static T? FromJson<T>(Stream json) where T : ISlackModel
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            T? toReturn = JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton);
            return toReturn;
        }

        public static async Task<T?> FromJsonAsync<T>(Stream json, JsonSerializerOptions? options = null) where T : ISlackModel
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));

            options ??= JsonConverterSettings.Singleton;
            T? toReturn = await JsonSerializer.DeserializeAsync<T>(json, options);
            return toReturn;
        }

    }

}