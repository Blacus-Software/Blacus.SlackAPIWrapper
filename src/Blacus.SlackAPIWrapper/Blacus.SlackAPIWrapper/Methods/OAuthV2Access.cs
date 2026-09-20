using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("oauth.v2.access", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class OAuthV2Access : SlackMethod
    {

        public static async Task<OAuthV2Access> ExecuteAsync(OptionalArguments slackOptionalArguments, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(slackOptionalArguments.ClientId) && !string.IsNullOrEmpty(slackOptionalArguments.ClientSecret))
            {
                var authToken = Encoding.ASCII.GetBytes($"{slackOptionalArguments.ClientId}:{slackOptionalArguments.ClientSecret}");
                return await SlackClient.HttpRequestAsync<OAuthV2Access>(new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken)), slackOptionalArguments, cancellationToken: cancellationToken);
            }

            return await SlackClient.HttpRequestAsync<OAuthV2Access>((AuthenticationHeaderValue)null!, slackOptionalArguments, cancellationToken: cancellationToken);
        }

        public static async Task<OAuthV2Access> ExecuteAsync(string token, OptionalArguments slackOptionalArguments, CancellationToken cancellationToken = default)
            => await SlackClient.HttpRequestAsync<OAuthV2Access>(token, slackOptionalArguments, cancellationToken: cancellationToken);


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

        public class OptionalArguments : SlackOptionalArguments
        {

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("client_id")]
            public string? ClientId { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("client_secret")]
            public string? ClientSecret { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("code")]
            public string? Code { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("code_verifier")]
            public string? CodeVerifier { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("redirect_uri")]
            public string? RedirectUri { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("grant_type")]
            public string? GrantType { get; set; }

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }

            #region Backward Compatibility Aliases

            [Obsolete("Use ClientId instead.")]
            [JsonIgnore]
            public string? client_id
            {
                get => ClientId;
                set => ClientId = value;
            }

            [Obsolete("Use ClientSecret instead.")]
            [JsonIgnore]
            public string? client_secret
            {
                get => ClientSecret;
                set => ClientSecret = value;
            }

            [Obsolete("Use Code instead.")]
            [JsonIgnore]
            public string? code
            {
                get => Code;
                set => Code = value;
            }

            [Obsolete("Use GrantType instead.")]
            [JsonIgnore]
            public string? grant_type
            {
                get => GrantType;
                set => GrantType = value;
            }

            [Obsolete("Use RedirectUri instead.")]
            [JsonIgnore]
            public string? redirect_uri
            {
                get => RedirectUri;
                set => RedirectUri = value;
            }

            [Obsolete("Use RefreshToken instead.")]
            [JsonIgnore]
            public string? refresh_token
            {
                get => RefreshToken;
                set => RefreshToken = value;
            }

            #endregion

        }

    }

    [Obsolete("Use OAuthV2Access.OptionalArguments instead.")]
    public class OptionalArguments : OAuthV2Access.OptionalArguments
    {
    }

}
