using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("users.identity", "Get", SlackMethodContentType.FormUrlEncoded)]
    public class UsersIdentity : SlackMethod
    {

        public static async Task<UsersIdentity> ExecuteAsync(string token)
              => await SlackClient.HttpRequestAsync<UsersIdentity>(token, null);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public User? User { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team")]
        public User? Team { get; set; }

    }

}
