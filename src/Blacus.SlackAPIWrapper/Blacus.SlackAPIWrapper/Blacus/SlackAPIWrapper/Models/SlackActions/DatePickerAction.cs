using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    public class DatePickerAction : SlackAction
    { 

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? SelectedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_date")]
        public string? SelectedDateString
        {
            get => SelectedDate.Value.ToString("yyyy-MM-dd");
            set => SelectedDate = DateTime.Parse(value);
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? InitialDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_date")]
        public string? InitialDateString
        {
            get => InitialDate.Value.ToString("yyyy-MM-dd");
            set => InitialDate = DateTime.Parse(value);
        }

    }

}