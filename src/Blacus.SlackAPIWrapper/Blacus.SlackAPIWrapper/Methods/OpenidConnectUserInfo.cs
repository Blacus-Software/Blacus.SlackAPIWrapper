using Blacus.SlackAPIWrapper.Attributes;
using System;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    [SlackMethod("openid.connect.userInfo", "Post", SlackMethodContentType.Json)]
    public class OpenidConnectUserInfo : SlackMethod
    {

        public static async Task<OpenidConnectUserInfo> ExecuteAsync(string token)
              => await SlackClient.HttpRequestAsync<OpenidConnectUserInfo>(token, new OptionalArguments());

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("sub")]
        public string Sub { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_id")]
        public string HttpsSlackComUserId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_id")]
        public string HttpsSlackComTeamId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("email_verified")]
        public bool? EmailVerified { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("date_email_verified")]
        public long? DateEmailVerified { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("picture")]
        public Uri Picture { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("given_name")]
        public string GivenName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("family_name")]
        public string FamilyName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("locale")]
        public string Locale { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_name")]
        public string HttpsSlackComTeamName { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_domain")]
        public string HttpsSlackComTeamDomain { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_24")]
        public string HttpsSlackComUserImage24 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_32")]
        public string HttpsSlackComUserImage32 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_48")]
        public string HttpsSlackComUserImage48 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_72")]
        public string HttpsSlackComUserImage72 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_192")]
        public string HttpsSlackComUserImage192 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_512")]
        public string HttpsSlackComUserImage512 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/user_image_1024")]
        public string HttpsSlackComUserImage1024 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_34")]
        public string HttpsSlackComTeamImage34 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_44")]
        public string HttpsSlackComTeamImage44 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_68")]
        public string HttpsSlackComTeamImage68 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_88")]
        public string HttpsSlackComTeamImage88 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_102")]
        public string HttpsSlackComTeamImage102 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_132")]
        public string HttpsSlackComTeamImage132 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_230")]
        public string HttpsSlackComTeamImage230 { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("https://slack.com/team_image_default")]
        public bool? HttpsSlackComTeamImageDefault { get; set; }

        public class OptionalArguments : SlackOptionalArguments
        {

        }

    }

}
