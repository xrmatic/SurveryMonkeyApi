using System;

namespace SurveyMonkeyApi.Throttling
{
    /// <summary>
    /// Exception thrown when the daily API request limit has been exceeded.
    /// </summary>
    public class DailyRateLimitExceededException : Exception
    {
        /// <summary>
        /// Gets the maximum number of requests allowed per day.
        /// </summary>
        public int DailyLimit { get; }

        /// <summary>
        /// Gets the current count of requests made today.
        /// </summary>
        public int CurrentCount { get; }

        public DailyRateLimitExceededException(int dailyLimit, int currentCount)
            : base($"Daily API request limit of {dailyLimit} has been reached. "
                 + $"Current count: {currentCount}. Limit resets at midnight UTC.")
        {
            DailyLimit = dailyLimit;
            CurrentCount = currentCount;
        }
    }
}
