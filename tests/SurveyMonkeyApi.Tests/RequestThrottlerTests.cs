using System;
using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Throttling;
using Xunit;

namespace SurveyMonkeyApi.Tests
{
    public class RequestThrottlerTests
    {
        [Fact]
        public void Constructor_ThrowsOnNegativeInterval()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RequestThrottler(TimeSpan.FromMilliseconds(-1)));
        }

        [Fact]
        public void Constructor_ThrowsOnZeroDailyLimit()
        {
            Assert.Throws<ArgumentOutOfRangeException>(
                () => new RequestThrottler(TimeSpan.Zero, 0));
        }

        [Fact]
        public async Task WaitAsync_IncrementsCounter()
        {
            var throttler = new RequestThrottler(TimeSpan.Zero);

            await throttler.WaitAsync();
            await throttler.WaitAsync();

            Assert.Equal(2, throttler.DailyRequestCount);
        }

        [Fact]
        public async Task WaitAsync_ThrowsWhenDailyLimitReached()
        {
            var throttler = new RequestThrottler(TimeSpan.Zero, dailyLimit: 2);

            await throttler.WaitAsync();
            await throttler.WaitAsync();

            await Assert.ThrowsAsync<DailyRateLimitExceededException>(
                () => throttler.WaitAsync());
        }

        [Fact]
        public async Task WaitAsync_EnforcesMinimumInterval()
        {
            var interval = TimeSpan.FromMilliseconds(100);
            var throttler = new RequestThrottler(interval, dailyLimit: 10);

            var before = DateTimeOffset.UtcNow;
            await throttler.WaitAsync();
            await throttler.WaitAsync();
            var elapsed = DateTimeOffset.UtcNow - before;

            // Allow 15ms tolerance for OS scheduling jitter
            var tolerance = TimeSpan.FromMilliseconds(15);
            Assert.True(elapsed >= interval - tolerance,
                $"Expected elapsed >= {interval - tolerance}, got {elapsed}");
        }

        [Fact]
        public async Task WaitAsync_RespectsDefaultDailyLimitOf500()
        {
            var throttler = new RequestThrottler(TimeSpan.Zero);

            for (int i = 0; i < 500; i++)
                await throttler.WaitAsync();

            await Assert.ThrowsAsync<DailyRateLimitExceededException>(
                () => throttler.WaitAsync());
        }

        [Fact]
        public async Task WaitAsync_RespectsCancellationToken()
        {
            var interval = TimeSpan.FromSeconds(10); // long interval so delay will fire
            var throttler = new RequestThrottler(interval, dailyLimit: 10);

            await throttler.WaitAsync(); // first call sets _lastRequestTime

            using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => throttler.WaitAsync(cts.Token));
        }

        [Fact]
        public void DailyRateLimitExceededException_ContainsCorrectValues()
        {
            var ex = new DailyRateLimitExceededException(500, 501);

            Assert.Equal(500, ex.DailyLimit);
            Assert.Equal(501, ex.CurrentCount);
            Assert.Contains("500", ex.Message);
        }
    }
}
