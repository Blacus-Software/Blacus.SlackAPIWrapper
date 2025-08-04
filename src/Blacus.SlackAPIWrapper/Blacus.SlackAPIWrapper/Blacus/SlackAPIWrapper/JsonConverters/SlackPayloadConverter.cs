using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

#pragma warning disable CS8603, CS8600, CS8602

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
                string payloadType = "test";

                using (var stream = new MemoryStream())
                {

                    Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                    jdoc.WriteTo(writer);
                    writer.Flush();
                    string json = Encoding.UTF8.GetString(stream.ToArray());

                    payloadType = fromJson<SlackPayload>(json).Type;

                    switch (payloadType)
                    {
                        case "block_actions":
                            toReturn = fromJson<SlackPayload_BlockAction>(json);
                            break;

                        case "view_submission":
                            toReturn = fromJson<SlackPayload_ViewSubmission>(json);
                            break;

                        case null:
                            toReturn = fromJson<SlackPayload_WebSocketCommand>(json);
                            break;

                        default:
                            toReturn = fromJson<SlackPayload_FallBack>(json);
                            break;

                    }

                }


                if (toReturn.ExtensionData != null)
                {

                    Console.WriteLine($"Payload Type: {payloadType}");

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

        private static T fromJson<T>(string json) where T : SlackModel
        {

            if (string.IsNullOrWhiteSpace(json))
                throw new NullReferenceException();

            T toReturn = JsonSerializer.Deserialize<T>(json, JsonConverterSettings.Singleton);

            toReturn.SourceJson = json;

            return toReturn;

        }

        public override void Write(Utf8JsonWriter writer, ISlackPayload value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public static readonly SlackPayloadConverter Singleton = new SlackPayloadConverter();

    }

}
