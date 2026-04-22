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
    /// Provides CRUD operations for SurveyMonkey collector messages.
    /// </summary>
    public class MessageService : SurveyMonkeyClientBase, IMessageService
    {
        public MessageService(
            HttpClient httpClient,
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(httpClient, throttler, baseUrl, accessToken) { }

        public MessageService(
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(throttler, baseUrl, accessToken) { }

        /// <summary>
        /// Convenience constructor that derives settings from <see cref="SurveyMonkeyOptions"/>.
        /// </summary>
        public MessageService(SurveyMonkeyOptions options)
            : this(
                new RequestThrottler(options.RequestInterval, options.DailyRequestLimit),
                options.BaseUrl,
                options.MessageAccessToken) { }

        /// <inheritdoc />
        public Task<PagedResponse<Message>> ListAsync(
            string collectorId,
            int page = 1,
            int perPage = 50,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            return HttpGetAsync<PagedResponse<Message>>(
                $"collectors/{collectorId}/messages?page={page}&per_page={perPage}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<Message> GetAsync(
            string collectorId,
            string messageId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
            return HttpGetAsync<Message>(
                $"collectors/{collectorId}/messages/{messageId}",
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<Message> CreateAsync(
            string collectorId,
            MessageRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPostAsync<Message>(
                $"collectors/{collectorId}/messages",
                request,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task<Message> UpdateAsync(
            string collectorId,
            string messageId,
            MessageRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPatchAsync<Message>(
                $"collectors/{collectorId}/messages/{messageId}",
                request,
                cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string collectorId,
            string messageId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(collectorId);
            ArgumentException.ThrowIfNullOrWhiteSpace(messageId);
            return HttpDeleteAsync(
                $"collectors/{collectorId}/messages/{messageId}",
                cancellationToken);
        }
    }
}
