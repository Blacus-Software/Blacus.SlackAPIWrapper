using Blacus.SlackAPIWrapper.Models;
using Blacus.SlackAPIWrapper.Models.SlackActions;
using Blacus.SlackAPIWrapper.Models.SlackPayloads;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.JsonConverters
{

    public class SlackActionConverter : JsonConverter<ISlackAction>
    {

        public override bool CanConvert(Type type) => typeof(ISlackAction).IsAssignableFrom(type);

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

            switch (payload.Type)
            {
                case ActionType.Button:
                    toReturn = ISlackModel.FromJson<ButtonAction>(json, JsonConverterSettingsSingleton);
                    break;

                case ActionType.DateTimePicker:
                    toReturn = ISlackModel.FromJson<DateTimePickerAction>(json, JsonConverterSettingsSingleton);
                    break;

                case ActionType.DatePicker:
                    toReturn = ISlackModel.FromJson<DatePickerAction>(json, JsonConverterSettingsSingleton);
                    break;

                case ActionType.TimePicker:
                    toReturn = ISlackModel.FromJson<TimePickerAction>(json, JsonConverterSettingsSingleton);
                    break;

                case ActionType.ConversationsSelect:
                    toReturn = ISlackModel.FromJson<ConversationsSelectAction>(json, JsonConverterSettingsSingleton);
                    break;

                case ActionType.StaticSelect:
                    toReturn = ISlackModel.FromJson<StaticSelectAction>(json, JsonConverterSettingsSingleton);
                    break;

                case ActionType.Checkboxes:
                    toReturn = ISlackModel.FromJson<CheckBoxAction>(json, JsonConverterSettingsSingleton);
                    break;

                default:
                    toReturn = ISlackModel.FromJson<SlackAction>(json, JsonConverterSettingsSingleton);
                    break;
            }

            if (toReturn.ExtensionData?.Count > 0)
            {
                var j = new JsonExtensionDataHelper(toReturn, "DateTimePickerAction");
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
