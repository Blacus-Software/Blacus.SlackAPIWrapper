using Blacus.SlackAPIWrapper.Helpers;
using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class DatetimePickerElement : BlockElement
    {

        public DatetimePickerElement(string actionId) : base(actionId) { }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? InitialDateTime { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_date_time")]
        public long? InitialDateTimeString
        {
            get => ((DateTimeOffset)InitialDateTime.Value).ToUnixTimeSeconds();
            set => InitialDateTime = value.Value.ToDateTimeFromUnixTimeStamp();
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("confirm")]
        public ConfirmationDialogObject? Confirm { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("focus_on_load")]
        public bool? FocusOnLoad { get; set; } 
    }

}