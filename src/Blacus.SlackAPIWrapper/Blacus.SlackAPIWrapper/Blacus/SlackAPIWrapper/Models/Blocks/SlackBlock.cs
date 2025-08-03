using System;
using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.Blocks
{

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(ActionBlock), typeDiscriminator: "actions")]
    [JsonDerivedType(typeof(ContextBlock), typeDiscriminator: "context")]
    [JsonDerivedType(typeof(DividerBlock), typeDiscriminator: "divider")]
    [JsonDerivedType(typeof(HeaderBlock), typeDiscriminator: "header")]
    [JsonDerivedType(typeof(InputBlock), typeDiscriminator: "input")]
    [JsonDerivedType(typeof(SectionBlock), typeDiscriminator: "section")]
    [JsonDerivedType(typeof(ImageBlock), typeDiscriminator: "image")]
    [JsonDerivedType(typeof(ShareShortcutBlock), typeDiscriminator: "share_shortcut")]
    [JsonDerivedType(typeof(RichTextSectionBlock), typeDiscriminator: "rich_text")]
    public class SlackBlock : SlackModel<SlackBlock>
    {

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("block_id")]
        public string BlockId { get; set; }

        public SlackBlock(string? blockId = null)
        {

            if (blockId == null)
            {
                BlockId = Guid.NewGuid().ToString();
            }
            else
            {
                BlockId = blockId;
            }

        }

    }

}