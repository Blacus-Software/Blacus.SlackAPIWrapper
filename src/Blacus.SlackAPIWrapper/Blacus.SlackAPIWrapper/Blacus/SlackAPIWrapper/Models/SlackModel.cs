using Blacus.SlackAPIWrapper.JsonConverters;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class SlackModel : ISlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string? SourceJson { get; set; }

        [JsonExtensionData]
        public Dictionary<string, JsonElement>? ExtensionData { get; set; }

        public string ToJson() =>
            System.Text.Json.JsonSerializer.Serialize<object>(this, JsonConverterSettings.Singleton);

    }

    public class SlackModel<T> : SlackModel where T : ISlackModel
    {

        public static T? FromJson(string json) =>
            ISlackModel.FromJson<T>(json);

        public static T? FromJson(System.IO.Stream json) =>
            ISlackModel.FromJson<T>(json);

    }

}