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
    /// Provides CRUD operations for SurveyMonkey survey responses.
    /// </summary>
    public class ResponseService : SurveyMonkeyClientBase, IResponseService
    {
        public ResponseService(
            HttpClient httpClient,
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(httpClient, throttler, baseUrl, accessToken) { }

        public ResponseService(
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(throttler, baseUrl, accessToken) { }

        /// <summary>
        /// Convenience constructor that derives settings from <see cref="SurveyMonkeyOptions"/>.
        /// </summary>
        public ResponseService(SurveyMonkeyOptions options)
            : this(
                new RequestThrottler(options.RequestInterval, options.DailyRequestLimit),
                options.BaseUrl,
                options.ResponseAccessToken) { }

        /// <inheritdoc />
        public Task<PagedResponse<SurveyResponse>> ListAsync(
            string surveyId,
            int page = 1,
            int perPage = 50,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            return HttpGetAsync<PagedResponse<SurveyResponse>>(
                $"surveys/{surveyId}/responses?page={page}&per_page={perPage}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<SurveyResponse> GetAsync(
            string surveyId,
            string responseId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            ArgumentException.ThrowIfNullOrWhiteSpace(responseId);
            return HttpGetAsync<SurveyResponse>(
                $"surveys/{surveyId}/responses/{responseId}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<SurveyResponse> CreateAsync(
            string surveyId,
            SurveyResponseRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPostAsync<SurveyResponse>(
                $"surveys/{surveyId}/responses",
                request,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<SurveyResponse> UpdateAsync(
            string surveyId,
            string responseId,
            SurveyResponseRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            ArgumentException.ThrowIfNullOrWhiteSpace(responseId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPatchAsync<SurveyResponse>(
                $"surveys/{surveyId}/responses/{responseId}",
                request,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string surveyId,
            string responseId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(surveyId);
            ArgumentException.ThrowIfNullOrWhiteSpace(responseId);
            return HttpDeleteAsync(
                $"surveys/{surveyId}/responses/{responseId}",
                cancellationToken);
        }
    }
}
