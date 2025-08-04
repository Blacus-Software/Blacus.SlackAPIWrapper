using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.SlackEvents;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class EventConverter : JsonConverter<ISlackPayloadEvent>
    {

        public override bool CanConvert(Type type) => typeof(ISlackPayloadEvent).IsAssignableFrom(type);

        public override ISlackPayloadEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {

            JsonDocument jdoc = JsonDocument.ParseValue(ref reader);

            using (var stream = new MemoryStream())
            {

                Utf8JsonWriter writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = true });
                jdoc.WriteTo(writer);
                writer.Flush();
                string json = Encoding.UTF8.GetString(stream.ToArray());

                var evt = fromJson<SlackEvent>(json);

                switch (evt.Type)
                {
                    case "app_mention":
                        return fromJson<EventAppMention>(json);

                    case "reaction_added":
                    case "reaction_removed":
                        return fromJson<EventReaction>(json);

                    default:
                        return fromJson<SlackEvent_Fallback>(json);
                }

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

        public override void Write(Utf8JsonWriter writer, ISlackPayloadEvent value, JsonSerializerOptions options) =>
            JsonSerializer.Serialize(writer, value, options);

        public static readonly EventConverter Singleton = new EventConverter();

    }

}