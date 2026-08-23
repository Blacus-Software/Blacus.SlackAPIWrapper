namespace Blacus.SlackAPIWrapper
{

    using Blacus.SlackAPIWrapper.Methods;
    using System.Net.Http.Headers;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for interacting with the Slack API using instance-based clients.
    /// </summary>
    public interface ISlackClient
    {

        /// <summary>
        /// Sends a request to the Slack API with the specified Bearer token and optional arguments.
        /// </summary>
        Task<T> SendAsync<T>(string token, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true, CancellationToken cancellationToken = default) where T : SlackMethod;

        /// <summary>
        /// Sends a request to the Slack API with the specified authentication header and optional arguments.
        /// </summary>
        Task<T> SendAsync<T>(AuthenticationHeaderValue authenticationHeaderValue, ISlackOptionalArguments? optionalArguments = null, bool streamJson = true, CancellationToken cancellationToken = default) where T : SlackMethod;

    }

}
