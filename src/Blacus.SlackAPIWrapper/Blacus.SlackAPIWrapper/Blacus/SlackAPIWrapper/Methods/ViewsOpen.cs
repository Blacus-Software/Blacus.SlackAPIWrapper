using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("views.open", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class ViewsOpen : SlackMethod
    {

        public static async Task<ViewsOpen> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<ViewsOpen>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("view")]
        public View? View { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("view")]
            public string? View { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("trigger_id")]
            public string? TriggerId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("interactivity_pointer")]
            public string? InteractivityPointer { get; set; }

        }

    }

}
