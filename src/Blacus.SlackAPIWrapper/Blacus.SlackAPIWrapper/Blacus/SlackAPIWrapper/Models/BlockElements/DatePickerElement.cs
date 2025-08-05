using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class DatePickerElement : BlockElement
    {

        private const int field_Placeholder_MaxLength = 150;

        public DatePickerElement(string actionId) : base(actionId) { }

        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public DateTime? InitialDate { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_date")]
        public string? InitialDateString
        {
            get => InitialDate.Value.ToString("yyyy-MM-dd");
            set => InitialDate = DateTime.Parse(value);
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("confirm")]
        public ConfirmationDialogObject? Confirm { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("focus_on_load")]
        public bool? FocusOnLoad { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("placeholder")]
        public TextObject? Placeholder
        {
            get { return placeholder; }
            set
            {
                placeholder = value;
                placeholder.MaxTextLength = field_Placeholder_MaxLength;
            }
        }
        private TextObject? placeholder;

    }

}