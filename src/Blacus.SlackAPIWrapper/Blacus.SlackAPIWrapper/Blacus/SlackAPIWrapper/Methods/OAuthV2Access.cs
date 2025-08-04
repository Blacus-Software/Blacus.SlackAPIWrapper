using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("oauth.v2.access", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class OAuthV2Access : SlackMethod
    {

        public static async Task<OAuthV2Access> ExecuteAsync(string token, OptionalArguments slackOptionalArguments)
              => await SlackClient.HttpRequestAsync<OAuthV2Access>(token, slackOptionalArguments);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("bot_user_id")]
        public string? BotUserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("app_id")]
        public string? AppId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team")]
        public Enterprise? Team { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise")]
        public Enterprise? Enterprise { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("authed_user")]
        public AuthedUser? AuthedUser { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("expires_in")]
        public long? ExpiresIn { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("refresh_token")]
        public string? RefreshToken { get; set; }

        public SlackCredential ToSlackCredential()
        {

            var toReturn = new SlackCredential()
            {
                //Scope = Scope,
                //BotUserId = BotUserId,
                AppId = AppId,
                //Team = Team,
                //Enterprise = new() { Id = Enterprise.Id, Name = Enterprise.Name },
                //AuthedUser = AuthedUser,
                //ExpiresIn = ExpiresIn,
                //RefreshToken = RefreshToken
            };

            switch (TokenType)
            {
                case "bot":
                    toReturn.BotUserToken = AccessToken;
                    break;
                case "user":
                    toReturn.UserToken = AccessToken;
                    break;
                case "app":
                    toReturn.AppToken = AccessToken;
                    break;
            }

            return toReturn;

        }

    }

    public class OptionalArguments : SlackOptionalArguments
    {

        public string? client_id { get; set; }
        public string? client_secret { get; set; }
        public string? code { get; set; }
        public string? grant_type { get; set; }
        public string? redirect_uri { get; set; }
        public string? refresh_token { get; set; }

    }

}
