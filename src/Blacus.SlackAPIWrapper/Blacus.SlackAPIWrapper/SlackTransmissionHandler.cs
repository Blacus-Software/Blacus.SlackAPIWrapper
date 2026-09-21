namespace Blacus.SlackAPIWrapper
{
    using System;
    using System.Threading.Tasks;

    public class SlackTransmissionHandler
    {

        /// <summary>
        /// Prepares the transmission and invokes the custom handler delegate, returning the resulting string.
        /// </summary>
        /// <param name="transmission">The incoming Slack ASP.NET transmission.</param>
        /// <param name="handler">The custom handler delegate to process the prepared transmission.</param>
        /// <returns>The string result returned by the custom handler.</returns>
        public static async Task<string?> HandleAsync(
            ISlackASPTransmission transmission,
            Func<ISlackASPTransmission, Task<string?>> handler)
        {

            if (transmission == null)
                throw new ArgumentNullException(nameof(transmission));

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            await transmission.PrepareAsync();

            return await handler(transmission);

        }

    }

}
