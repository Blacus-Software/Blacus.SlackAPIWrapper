using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    public interface ISlackAction : ISlackModel
    {

        public ActionType? Type { get; }
        public string? ActionId { get; set; }
        public string? BlockId { get; set; }

        public static ActionType ConversionType { get; set; }

    }

    public enum ActionType
    {
        Button,
        DateTimePicker,
        TimePicker,
        DatePicker,
        ConversationsSelect,
        StaticSelect,
        Checkboxes,
        ChannelsSelect
    }

    public class SlackAction : SlackModel, ISlackAction
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public ActionType? Type
        {
            get
            {

                if (string.IsNullOrWhiteSpace(TypeString))
                    return null;

                return Enum.Parse<ActionType>(TypeString.Replace("_", string.Empty), true);

            }
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("action_ts")]
        public string? ActionTs { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? TypeString { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("action_id")]
        public string? ActionId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("block_id")]
        public string? BlockId { get; set; }

    }

}