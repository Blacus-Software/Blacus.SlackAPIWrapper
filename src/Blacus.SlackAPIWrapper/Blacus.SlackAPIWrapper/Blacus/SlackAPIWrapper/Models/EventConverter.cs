using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{
    public class EventConverterJsonConverter : JsonConverter<ISlackPayload_EventCallback_Event>
    {

        public override bool CanConvert(Type t) => true;// t == typeof(ISlackPayload_EventCallback_Event);

        public override ISlackPayload_EventCallback_Event Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
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
                        return fromJson<SlackEvent_AppMention>(json);

                    case "reaction_removed":
                        return fromJson<SlackEvent_ReactionRemoved>(json);

                    case "reaction_added":
                        return fromJson<SlackEvent_ReactionAdded>(json);

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

        public override void Write(Utf8JsonWriter writer, ISlackPayload_EventCallback_Event value, JsonSerializerOptions options)
        {

            return;
        }

        public static readonly EventConverterJsonConverter Singleton = new EventConverterJsonConverter();

    }
}