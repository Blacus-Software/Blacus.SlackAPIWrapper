using Blacus.SlackAPIWrapper.Helpers;
using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    public class DateTimePickerAction : SlackAction
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? SelectedDateTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_date_time")]
        public long? SelectedDateTimeValue
        {
            get => ((DateTimeOffset)SelectedDateTime.Value).ToUnixTimeSeconds();
            set => SelectedDateTime = value.Value.ToDateTimeFromUnixTimeStamp();
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? InitialDateTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_date_time")]
        public long? InitialDateTimeValue
        {
            get => ((DateTimeOffset)InitialDateTime.Value).ToUnixTimeSeconds();
            set => InitialDateTime = value.Value.ToDateTimeFromUnixTimeStamp();
        }

    }

}