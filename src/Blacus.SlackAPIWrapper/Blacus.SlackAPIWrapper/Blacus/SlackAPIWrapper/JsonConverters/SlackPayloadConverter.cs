using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class SlackPayloadConverter : JsonConverter<ISlackPayload>
    {

        public override bool CanConvert(Type type) => typeof(ISlackPayload).IsAssignableFrom(type);

        public override ISlackPayload? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {

                ISlackPayload toReturn = null;

                JsonDocument jdoc = JsonDocument.ParseValue(ref reader);
                string json = null;
                SlackPayload payload = null;

                using (var stream = new MemoryStream())
                {

                    Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                    jdoc.WriteTo(writer);
                    writer.Flush();
                    json = Encoding.UTF8.GetString(stream.ToArray());

                }

                payload = ISlackModel.FromJson<SlackPayload>(json, JsonConverterSettingsSingleton);

                switch (payload.Type)
                {
                    case PayloadType.BlockActions:
                        toReturn = ISlackModel.FromJson<SlackPayload_BlockAction>(json, JsonConverterSettingsSingleton);
                        break;

                    case PayloadType.ViewClosed:
                    case PayloadType.ViewSubmission:
                        toReturn = ISlackModel.FromJson<SlackPayload_ViewSubmission>(json, JsonConverterSettingsSingleton);
                        break;

                    case PayloadType.UrlVerification:
                        toReturn = ISlackModel.FromJson<SlackPayload_UrlResponse>(json, JsonConverterSettingsSingleton);
                        break;

                    case null:
                    case PayloadType.SlashCommand:
                        toReturn = ISlackModel.FromJson<SlackPayload_Command>(json, JsonConverterSettingsSingleton);
                        break;

                    default:
                        toReturn = ISlackModel.FromJson<SlackPayload_FallBack>(json, JsonConverterSettingsSingleton);
                        break;

                }


                if (toReturn.ExtensionData != null)
                {

                    Console.WriteLine($"Payload Type: {payload.Type}");

                    foreach (var item in toReturn.ExtensionData)
                    {
                        Console.WriteLine(item.Key);
                    }

                    Console.WriteLine();

                }

                return toReturn;

            }
            catch (Exception ex)
            {

                _ = ex.ToString();
                throw;
            }

        }

        public override void Write(Utf8JsonWriter writer, ISlackPayload value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize<object>(writer, value, new JsonSerializerOptions()
            {
                WriteIndented = true,
                Converters =
                {
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase),
                    BoolConverter.Singleton,
                    SlackActionConverter.Singleton,
                }
            });

        public static readonly SlackPayloadConverter Singleton = new SlackPayloadConverter();

        private static JsonSerializerOptions JsonConverterSettingsSingleton { get => jsonConverterSettings.Value; }
        private static Lazy<JsonSerializerOptions> jsonConverterSettings = new Lazy<JsonSerializerOptions>(() =>
        {

            var option = JsonConverterSettings.Generate();
            var converter = option.Converters.SingleOrDefault(conv => conv.GetType() == typeof(SlackPayloadConverter));
            option.Converters.Remove(converter);
            return option;

        });

    }

}
