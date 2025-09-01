using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.SlackActions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class SlackActionConverter : JsonConverter<ISlackAction>
    {

        public override bool CanConvert(Type type) => typeof(ISlackAction).IsAssignableFrom(type);

        private static Dictionary<ActionType, Type> actionTypes = typeof(ISlackAction).Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ISlackAction).IsAssignableFrom(t) && t.GetCustomAttributes<SlackActionAttribute>().Any())
            .Select(t => new { Type = t, Attribute = t.GetCustomAttributes<SlackActionAttribute>().FirstOrDefault() })
            .Where(t => t.Attribute != null)
            .ToDictionary(t => t.Attribute.ActionType, t => t.Type);

        public override ISlackAction? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            ISlackAction toReturn = null;

            JsonDocument jdoc = JsonDocument.ParseValue(ref reader);
            string json = null;
            ISlackAction payload = null;

            using (var stream = new MemoryStream())
            {

                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jdoc.WriteTo(writer);
                writer.Flush();
                json = Encoding.UTF8.GetString(stream.ToArray());

            }

            payload = ISlackModel.FromJson<SlackAction>(json, JsonConverterSettingsSingleton);

            toReturn = (ISlackAction?)System.Text.Json.JsonSerializer.Deserialize(
                json: json,
                returnType: actionTypes.SingleOrDefault(act => act.Key == payload.Type).Value,
                options: JsonConverterSettingsSingleton);

            if (toReturn.ExtensionData?.Count > 0)
            {
                var j = new JsonExtensionDataHelper(toReturn, payload.Type.ToString());
                Console.WriteLine(j.GetAllClassStrings());
            }

            return toReturn;

        }

        public override void Write(Utf8JsonWriter writer, ISlackAction value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public static readonly SlackActionConverter Singleton = new SlackActionConverter();

        private static JsonSerializerOptions JsonConverterSettingsSingleton { get => jsonConverterSettings.Value; }
        private static Lazy<JsonSerializerOptions> jsonConverterSettings = new Lazy<JsonSerializerOptions>(() =>
        {

            var option = JsonConverterSettings.Generate();
            var converter = option.Converters.SingleOrDefault(conv => conv.GetType() == typeof(SlackActionConverter));
            option.Converters.Remove(converter);
            return option;

        });

    }

}
