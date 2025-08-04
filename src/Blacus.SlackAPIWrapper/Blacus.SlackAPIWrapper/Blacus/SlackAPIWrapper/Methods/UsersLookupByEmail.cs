using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("users.lookupByEmail", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class UsersLookupByEmail : SlackMethod
    {

        public static async Task<UsersLookupByEmail> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<UsersLookupByEmail>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public User? User { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("email")]
            public string? Email { get; set; }

        }

    }

}
