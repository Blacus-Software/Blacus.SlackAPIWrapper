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

    [SlackMethod("oauth.v2.user.access", "Post", SlackMethodContentType.FormUrlEncoded)]
    public class OAuthV2UserAccess : SlackMethod
    {

        public static async Task<OAuthV2UserAccess> ExecuteAsync(OptionalArguments slackOptionalArguments, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(slackOptionalArguments.ClientId) && !string.IsNullOrEmpty(slackOptionalArguments.ClientSecret))
            {
                var authToken = Encoding.ASCII.GetBytes($"{slackOptionalArguments.ClientId}:{slackOptionalArguments.ClientSecret}");
                return await SlackClient.HttpRequestAsync<OAuthV2UserAccess>(new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken)), slackOptionalArguments, cancellationToken: cancellationToken);
            }

            return await SlackClient.HttpRequestAsync<OAuthV2UserAccess>((AuthenticationHeaderValue)null!, slackOptionalArguments, cancellationToken: cancellationToken);
        }

        public static async Task<OAuthV2UserAccess> ExecuteAsync(string token, OptionalArguments slackOptionalArguments, CancellationToken cancellationToken = default)
              => await SlackClient.HttpRequestAsync<OAuthV2UserAccess>(token, slackOptionalArguments, cancellationToken: cancellationToken);

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("id_token")]
        public string? IdToken { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("scope")]
        public string? Scope { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("authed_user")]
        public AuthedUser? AuthedUser { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("team")]
        public Enterprise? Team { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("enterprise")]
        public Enterprise? Enterprise { get; set; }

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

            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("assertion")]
            public string? Assertion { get; set; }

        }

    }

}
