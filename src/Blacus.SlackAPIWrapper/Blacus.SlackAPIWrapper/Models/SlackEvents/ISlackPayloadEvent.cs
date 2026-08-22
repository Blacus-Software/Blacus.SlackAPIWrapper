using System.Text.Json.Serialization;

namespace Blacus.SlackAPIWrapper.Models.SlackEvents
{
     
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type", UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
    [JsonDerivedType(typeof(EventAppMention), typeDiscriminator: "app_mention")]
    [JsonDerivedType(typeof(EventReactionAdded), typeDiscriminator: "reaction_added")]
    [JsonDerivedType(typeof(EventReactionRemoved), typeDiscriminator: "reaction_removed")]
    [JsonDerivedType(typeof(EventMessage), typeDiscriminator: "message")]
    public interface ISlackPayloadEvent : ISlackModel;

}