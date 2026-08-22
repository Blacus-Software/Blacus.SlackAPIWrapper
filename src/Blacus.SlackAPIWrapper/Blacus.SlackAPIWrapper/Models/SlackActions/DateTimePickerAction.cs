using Blacus.SlackAPIWrapper.Helpers;
using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    public class DateTimePickerAction : SlackAction
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [JsonPropertyName("selected_date_time")]
        public DateTimeOffset? SelectedDateTime { get; set; } 

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        [JsonPropertyName("initial_date_time")]
        public DateTimeOffset? InitialDateTime { get; set; } 

    }

}