using Blacus.SlackAPIWrapper.Helpers;
using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models
{

    public class ViewStateValuesOptions : SlackModel
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_date")]
        public DateOnly? SelectedDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_time")]
        public string? SelectedTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_options")]
        public OptionObject[]? SelectedOptions { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_option")]
        public OptionObject? SelectedOption { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("actionId")]
        public TextObject? ActionId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? SelectedDateTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("selected_date_time")]
        public string? SelectedDateTimeString
        {
            get => ((DateTimeOffset)SelectedDateTime.Value).ToUnixTimeSeconds().ToString();
            set
            {
                if (value != null)
                    SelectedDateTime = long.Parse(value).ToDateTimeFromUnixTimeStamp();
            }
        }

    }

}