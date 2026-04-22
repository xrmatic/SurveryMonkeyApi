using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Throttling;

namespace SurveyMonkeyApi.Client
{
    /// <summary>
    /// Base HTTP client that attaches a Bearer token, enforces throttling, and
    /// deserializes JSON responses for a single SurveyMonkey resource type.
    /// </summary>
    public abstract class SurveyMonkeyClientBase : IDisposable
    {
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        private readonly HttpClient _httpClient;
        private readonly bool _ownsHttpClient;
        private readonly IRequestThrottler _throttler;
        private bool _disposed;

        /// <summary>
        /// Creates a new client using the provided <paramref name="httpClient"/> and
        /// <paramref name="throttler"/>. The <paramref name="baseUrl"/> and
        /// <paramref name="accessToken"/> are applied to every request.
        /// </summary>
        protected SurveyMonkeyClientBase(
            HttpClient httpClient,
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
        {
            ArgumentNullException.ThrowIfNull(httpClient);
            ArgumentNullException.ThrowIfNull(throttler);
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("Base URL must not be empty.", nameof(baseUrl));
            if (string.IsNullOrWhiteSpace(accessToken))
                throw new ArgumentException("Access token must not be empty.", nameof(accessToken));

            _httpClient = httpClient;
            _ownsHttpClient = false;
            _throttler = throttler;

            _httpClient.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);
            _httpClient.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Creates a new client that owns its own <see cref="HttpClient"/> instance.
        /// </summary>
        protected SurveyMonkeyClientBase(
            IRequestThrottler throttler,
            string baseUrl,
            string accessToken)
            : this(new HttpClient(), throttler, baseUrl, accessToken)
        {
            _ownsHttpClient = true;
        }

        // ── HTTP helpers ──────────────────────────────────────────────────────

        protected async Task<T> HttpGetAsync<T>(
            string relativeUrl,
            CancellationToken cancellationToken = default)
        {
            await _throttler.WaitAsync(cancellationToken).ConfigureAwait(false);
            var response = await _httpClient
                .GetAsync(relativeUrl, cancellationToken)
                .ConfigureAwait(false);
            return await ReadAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }

        protected async Task<T> HttpPostAsync<T>(
            string relativeUrl,
            object body,
            CancellationToken cancellationToken = default)
        {
            await _throttler.WaitAsync(cancellationToken).ConfigureAwait(false);
            var content = Serialize(body);
            var response = await _httpClient
                .PostAsync(relativeUrl, content, cancellationToken)
                .ConfigureAwait(false);
            return await ReadAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }

        protected async Task<T> HttpPatchAsync<T>(
            string relativeUrl,
            object body,
            CancellationToken cancellationToken = default)
        {
            await _throttler.WaitAsync(cancellationToken).ConfigureAwait(false);
            var content = Serialize(body);
            var request = new HttpRequestMessage(HttpMethod.Patch, relativeUrl) { Content = content };
            var response = await _httpClient
                .SendAsync(request, cancellationToken)
                .ConfigureAwait(false);
            return await ReadAsync<T>(response, cancellationToken).ConfigureAwait(false);
        }

        protected async Task HttpDeleteAsync(
            string relativeUrl,
            CancellationToken cancellationToken = default)
        {
            await _throttler.WaitAsync(cancellationToken).ConfigureAwait(false);
            var response = await _httpClient
                .DeleteAsync(relativeUrl, cancellationToken)
                .ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        // ── Private helpers ───────────────────────────────────────────────────

        private static StringContent Serialize(object body)
        {
            var json = JsonSerializer.Serialize(body, _jsonOptions);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        private static async Task<T> ReadAsync<T>(
            HttpResponseMessage response,
            CancellationToken cancellationToken)
        {
            response.EnsureSuccessStatusCode();
            var stream = await response.Content
                .ReadAsStreamAsync(cancellationToken)
                .ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(stream, _jsonOptions)
                ?? throw new InvalidOperationException("API returned a null or empty response.");
        }

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing && _ownsHttpClient)
                _httpClient.Dispose();
            _disposed = true;
        }
    }
}
