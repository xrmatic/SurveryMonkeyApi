using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Client;
using SurveyMonkeyApi.Models;
using SurveyMonkeyApi.Throttling;

namespace SurveyMonkeyApi.Services
{
    /// <summary>
    /// Provides CRUD operations for SurveyMonkey collectors.
    /// </summary>
    public class CollectorService : SurveyMonkeyClientBase, ICollectorService
    {
        public CollectorService(
            HttpClient httpClient,
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(httpClient, throttler, baseUrl, accessToken) { }

        public CollectorService(
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(throttler, baseUrl, accessToken) { }

        /// <summary>
        /// Convenience constructor that derives settings from <see cref="SurveyMonkeyOptions"/>.
        /// </summary>
        public CollectorService(SurveyMonkeyOptions options)
            : this(
                new RequestThrottler(options.RequestInterval, options.DailyRequestLimit),
                options.BaseUrl,
                options.CollectorAccessToken) { }

        /// <inheritdoc />
        public Task<PagedResponse<Collector>> ListAsync(
            string surveyId,
            int page = 1,
            int perPage = 50,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            return HttpGetAsync<PagedResponse<Collector>>(
                $"surveys/{surveyId}/collectors?page={page}&per_page={perPage}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<Collector> GetAsync(
            string collectorId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            return HttpGetAsync<Collector>($"collectors/{collectorId}", cancellationToken);
        }

        /// <inheritdoc />
        public Task<Collector> CreateAsync(
            string surveyId,
            CollectorRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPostAsync<Collector>($"surveys/{surveyId}/collectors", request, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Collector> UpdateAsync(
            string collectorId,
            CollectorRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPatchAsync<Collector>($"collectors/{collectorId}", request, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string collectorId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            return HttpDeleteAsync($"collectors/{collectorId}", cancellationToken);
        }
    }
}
