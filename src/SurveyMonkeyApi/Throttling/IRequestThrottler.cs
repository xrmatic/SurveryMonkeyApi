using System;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyMonkeyApi.Throttling
{
    /// <summary>
    /// Controls the rate and daily volume of requests made using a single API access token.
    /// </summary>
    public interface IRequestThrottler
    {
        /// <summary>
        /// Waits for the configured interval and checks the daily request limit before
        /// allowing the next request to proceed.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A task that completes when the request is allowed to proceed.</returns>
        /// <exception cref="DailyRateLimitExceededException">
        /// Thrown when the daily request limit has been reached.
        /// </exception>
        Task WaitAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the number of requests made today for this throttler.
        /// </summary>
        int DailyRequestCount { get; }
    }
}
