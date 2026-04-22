using System;
using System.Threading;
using System.Threading.Tasks;

namespace SurveyMonkeyApi.Throttling
{
    /// <summary>
    /// Throttles API requests by enforcing a minimum interval between requests and
    /// a configurable daily limit per access token.
    /// </summary>
    public class RequestThrottler : IRequestThrottler
    {
        private readonly TimeSpan _requestInterval;
        private readonly int _dailyLimit;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private int _dailyRequestCount;
        private DateTime _currentDay;
        private DateTimeOffset _lastRequestTime = DateTimeOffset.MinValue;

        /// <summary>
        /// Initializes a new <see cref="RequestThrottler"/>.
        /// </summary>
        /// <param name="requestInterval">
        /// Minimum time to wait between consecutive requests.
        /// </param>
        /// <param name="dailyLimit">
        /// Maximum number of requests permitted per calendar day (UTC). Defaults to 500.
        /// </param>
        public RequestThrottler(TimeSpan requestInterval, int dailyLimit = 500)
        {
            if (requestInterval < TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(requestInterval),
                    "Request interval must not be negative.");

            if (dailyLimit <= 0)
                throw new ArgumentOutOfRangeException(nameof(dailyLimit),
                    "Daily limit must be greater than zero.");

            _requestInterval = requestInterval;
            _dailyLimit = dailyLimit;
            _currentDay = DateTime.UtcNow.Date;
        }

        /// <inheritdoc />
        public int DailyRequestCount => _dailyRequestCount;

        /// <inheritdoc />
        public async Task WaitAsync(CancellationToken cancellationToken = default)
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                ResetCounterIfNewDay();

                if (_dailyRequestCount >= _dailyLimit)
                    throw new DailyRateLimitExceededException(_dailyLimit, _dailyRequestCount);

                var elapsed = DateTimeOffset.UtcNow - _lastRequestTime;
                if (elapsed < _requestInterval)
                {
                    var delay = _requestInterval - elapsed;
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }

                _lastRequestTime = DateTimeOffset.UtcNow;
                Interlocked.Increment(ref _dailyRequestCount);
            }
            finally
            {
                _semaphore.Release();
            }
        }

        private void ResetCounterIfNewDay()
        {
            var today = DateTime.UtcNow.Date;
            if (today > _currentDay)
            {
                _currentDay = today;
                Interlocked.Exchange(ref _dailyRequestCount, 0);
            }
        }
    }
}
