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
    /// Provides CRUD operations for SurveyMonkey surveys.
    /// </summary>
    public class SurveyService : SurveyMonkeyClientBase, ISurveyService
    {
        public SurveyService(
            HttpClient httpClient,
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(httpClient, throttler, baseUrl, accessToken) { }

        public SurveyService(
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(throttler, baseUrl, accessToken) { }

        /// <summary>
        /// Convenience constructor that derives settings from <see cref="SurveyMonkeyOptions"/>.
        /// </summary>
        public SurveyService(SurveyMonkeyOptions options)
            : this(
                new RequestThrottler(options.RequestInterval, options.DailyRequestLimit),
                options.BaseUrl,
                options.SurveyAccessToken) { }

        /// <inheritdoc />
        public Task<PagedResponse<Survey>> ListAsync(
            int page = 1,
            int perPage = 50,
            CancellationToken cancellationToken = default)
            => HttpGetAsync<PagedResponse<Survey>>(
                $"surveys?page={page}&per_page={perPage}",
                cancellationToken);

        /// <inheritdoc />
        public Task<Survey> GetAsync(
            string surveyId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            return HttpGetAsync<Survey>($"surveys/{surveyId}", cancellationToken);
        }

        /// <inheritdoc />
        public Task<Survey> CreateAsync(
            SurveyRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            return HttpPostAsync<Survey>("surveys", request, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Survey> UpdateAsync(
            string surveyId,
            SurveyRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPatchAsync<Survey>($"surveys/{surveyId}", request, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string surveyId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            return HttpDeleteAsync($"surveys/{surveyId}", cancellationToken);
        }
    }
}
