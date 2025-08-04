using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.BlockElements
{

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(ButtonElement), typeDiscriminator: "button")]
    [JsonDerivedType(typeof(DatePickerElement), typeDiscriminator: "datepicker")]
    [JsonDerivedType(typeof(DatetimePickerElement), typeDiscriminator: "datetimepicker")]
    [JsonDerivedType(typeof(EmailInputElement), typeDiscriminator: "email_text_input")]
    [JsonDerivedType(typeof(ImageElement), typeDiscriminator: "image")]
    [JsonDerivedType(typeof(NumberInputElement), typeDiscriminator: "number_input")]
    [JsonDerivedType(typeof(OverflowMenuElement), typeDiscriminator: "overflow")]
    [JsonDerivedType(typeof(PlainTextInputElement), typeDiscriminator: "plain_text_input")]
    [JsonDerivedType(typeof(RadioButtonGroupElement), typeDiscriminator: "radio_buttons")]
    [JsonDerivedType(typeof(TimePickerElement), typeDiscriminator: "timepicker")]
    [JsonDerivedType(typeof(URLInputElement), typeDiscriminator: "url_text_input")]
    [JsonDerivedType(typeof(RichTextSectionElement), typeDiscriminator: "rich_text_section")]
    [JsonDerivedType(typeof(RichTextListElement), typeDiscriminator: "rich_text_list")]
    [JsonDerivedType(typeof(RichTextQuoteElement), typeDiscriminator: "rich_text_quote")]
    [JsonDerivedType(typeof(TextElement), typeDiscriminator: "text")]
    [JsonDerivedType(typeof(UserElement), typeDiscriminator: "user")]
    [JsonDerivedType(typeof(LinkElement), typeDiscriminator: "link")]
    [JsonDerivedType(typeof(ChannelElement), typeDiscriminator: "Channel")]
    [JsonDerivedType(typeof(EmojiElement), typeDiscriminator: "emoji")]
    [JsonDerivedType(typeof(BroadcastElement), typeDiscriminator: "broadcast")]
    [JsonDerivedType(typeof(CheckboxElement), typeDiscriminator: "checkboxes")]
    [JsonDerivedType(typeof(ExternalSelect), typeDiscriminator: "external_select")]
    [JsonDerivedType(typeof(UsersSelect), typeDiscriminator: "users_select")]
    [JsonDerivedType(typeof(ConversationsSelect), typeDiscriminator: "conversations_select")]
    [JsonDerivedType(typeof(ChannelsSelect), typeDiscriminator: "channels_select")]
    [JsonDerivedType(typeof(SelectMenuElement), typeDiscriminator: "static_select")]
    public class BlockElement : SlackModel<BlockElement>, IBlockElement
    {

        [JsonPropertyOrder(1)]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("action_id")]
        public string? ActionId { get; set; }

        public BlockElement(string? actionId) =>
            ActionId = actionId;

    }

    public interface IBlockElement
    {

    }

}