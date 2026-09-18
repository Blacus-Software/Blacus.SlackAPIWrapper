using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackEvents
{
     
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(EventAppMention), typeDiscriminator: "app_mention")]
    [JsonDerivedType(typeof(EventReactionAdded), typeDiscriminator: "reaction_added")]
    [JsonDerivedType(typeof(EventReactionRemoved), typeDiscriminator: "reaction_removed")]
    [JsonDerivedType(typeof(EventMessage), typeDiscriminator: "message")]
    [JsonDerivedType(typeof(EventAppHomeOpened), typeDiscriminator: "app_home_opened")]
    public interface ISlackPayloadEvent : ISlackModel;

}