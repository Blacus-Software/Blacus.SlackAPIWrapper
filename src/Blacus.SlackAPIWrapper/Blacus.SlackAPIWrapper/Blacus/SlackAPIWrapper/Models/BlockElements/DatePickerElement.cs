using System;
using System.Text.Json.Serialization;

#pragma warning disable CS8601, CS8603, CS8600, CS8602, CS1998, CS8625, CS8618, CS8604

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    public class DatePickerElement : BlockElement
    {

        private const int field_Placeholder_MaxLength = 150;

        public DatePickerElement(string actionId) : base(actionId) { }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("initial_date")]
        public DateTime? InitialDate { get; set; }

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
                placeholder.ThrowExceptionWhenNull = true;
                placeholder.MaxTextLength = field_Placeholder_MaxLength;
            }
        }
        private TextObject? placeholder;

    }

}