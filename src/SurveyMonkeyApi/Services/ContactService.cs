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
    /// Provides CRUD operations for SurveyMonkey contacts.
    /// </summary>
    public class ContactService : SurveyMonkeyClientBase, IContactService
    {
        public ContactService(
            HttpClient httpClient,
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(httpClient, throttler, baseUrl, accessToken) { }

        public ContactService(
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : base(throttler, baseUrl, accessToken) { }

        /// <summary>
        /// Convenience constructor that derives settings from <see cref="SurveyMonkeyOptions"/>.
        /// </summary>
        public ContactService(SurveyMonkeyOptions options)
            : this(
                new RequestThrottler(options.RequestInterval, options.DailyRequestLimit),
                options.BaseUrl,
                options.ContactAccessToken) { }

        /// <inheritdoc />
        public Task<PagedResponse<Contact>> ListAsync(
            int page = 1,
            int perPage = 50,
            CancellationToken cancellationToken = default)
            => HttpGetAsync<PagedResponse<Contact>>(
                $"contacts?page={page}&per_page={perPage}",
                cancellationToken);

        /// <inheritdoc />
        public Task<Contact> GetAsync(
            string contactId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(contactId);
            return HttpGetAsync<Contact>($"contacts/{contactId}", cancellationToken);
        }

        /// <inheritdoc />
        public Task<Contact> CreateAsync(
            ContactRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            return HttpPostAsync<Contact>("contacts", request, cancellationToken);
        }

        /// <inheritdoc />
        public Task<Contact> UpdateAsync(
            string contactId,
            ContactRequest request,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(contactId);
            ArgumentNullException.ThrowIfNull(request);
            return HttpPatchAsync<Contact>($"contacts/{contactId}", request, cancellationToken);
        }

        /// <inheritdoc />
        public Task DeleteAsync(
            string contactId,
            CancellationToken cancellationToken = default)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(contactId);
            return HttpDeleteAsync($"contacts/{contactId}", cancellationToken);
        }
    }
}
