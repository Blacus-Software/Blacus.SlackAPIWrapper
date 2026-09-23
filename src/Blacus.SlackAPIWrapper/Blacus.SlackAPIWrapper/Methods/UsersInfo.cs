using Blacus.SlackAPIWrapper.Attributes;
using Blacus.SlackAPIWrapper.Models;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Blacus.SlackAPIWrapper.Methods
{

    /// <summary>
    /// Gets information about a Slack user.
    /// <see href="https://docs.slack.dev/reference/methods/users.info/"/>
    /// </summary>
    [SlackLimiterTier(LimiterTier.WebAPITier4)]
    [SlackMethod("users.info", "POST", SlackMethodContentType.FormUrlEncoded)]
    public class UsersInfo : SlackMethod
    {

        /// <summary>
        /// Executes the users.info method using the specified token and optional arguments.
        /// </summary>
        public static async Task<UsersInfo> ExecuteAsync(string token, OptionalArguments? slackOptionalArguments = null, CancellationToken cancellationToken = default)
              => await SlackClient.HttpRequestAsync<UsersInfo>(token, slackOptionalArguments, cancellationToken: cancellationToken);

        /// <summary>
        /// The user object returned by Slack.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [JsonPropertyName("user")]
        public User? User { get; set; }

        /// <summary>
        /// Request arguments for users.info.
        /// </summary>
        public class OptionalArguments : SlackOptionalArguments
        {

            /// <summary>
            /// User ID to get info on.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("user")]
            public string? User { get; set; }

            /// <summary>
            /// Convenience alias for <see cref="User"/> property.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
            public string? UserId
            {
                get => User;
                set => User = value;
            }

            /// <summary>
            /// Set this to true to receive the locale for this user. Defaults to false.
            /// </summary>
            [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
            [JsonPropertyName("include_locale")]
            public bool? IncludeLocale { get; set; }

        }

    }

}
