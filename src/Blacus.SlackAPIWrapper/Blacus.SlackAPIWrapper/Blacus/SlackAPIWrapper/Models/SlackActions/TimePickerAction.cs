using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    [SlackAction(ActionType.TimePicker)]
    public class TimePickerAction : SlackAction
    {

        const string dateFormat = "HH:mm";

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? SelectedTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_time")]
        public string? SelectedTimeString
        {
            get => SelectedTime.Value.ToString(dateFormat);
            set => SelectedTime = DateTime.Parse(value);
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? InitialTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_time")]
        public string? InitialTimeString
        {
            get => InitialTime.Value.ToString(dateFormat);
            set => InitialTime = DateTime.Parse(value);
        }

    }

}