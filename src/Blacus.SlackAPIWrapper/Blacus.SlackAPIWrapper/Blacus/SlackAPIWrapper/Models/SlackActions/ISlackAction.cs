using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackActions
{

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(ButtonAction), typeDiscriminator: "button")]
    [JsonDerivedType(typeof(ChannelsSelectAction), typeDiscriminator: "channels_select")]
    [JsonDerivedType(typeof(CheckBoxAction), typeDiscriminator: "checkboxes")]
    [JsonDerivedType(typeof(ConversationsSelectAction), typeDiscriminator: "conversations_select")]
    [JsonDerivedType(typeof(DatePickerAction), typeDiscriminator: "datepicker")]
    [JsonDerivedType(typeof(DateTimePickerAction), typeDiscriminator: "datetimepicker")]
    [JsonDerivedType(typeof(StaticSelectAction), typeDiscriminator: "static_select")]
    [JsonDerivedType(typeof(TimePickerAction), typeDiscriminator: "timepicker")]
    public interface ISlackAction : ISlackModel
    {

        public string? ActionId { get; set; }

        public string? BlockId { get; set; }

    }

}
