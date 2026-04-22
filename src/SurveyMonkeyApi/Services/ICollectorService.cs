using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Models;

namespace SurveyMonkeyApi.Services
{
    /// <summary>
    /// CRUD operations for SurveyMonkey collectors.
    /// </summary>
    public interface ICollectorService
    {
        /// <summary>Returns a paged list of collectors for the specified survey.</summary>
        Task<PagedResponse<Collector>> ListAsync(string surveyId, int page = 1, int perPage = 50, CancellationToken cancellationToken = default);

        /// <summary>Returns details for a single collector.</summary>
        Task<Collector> GetAsync(string collectorId, CancellationToken cancellationToken = default);

        /// <summary>Creates a new collector for the specified survey.</summary>
        Task<Collector> CreateAsync(string surveyId, CollectorRequest request, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing collector (partial update).</summary>
        Task<Collector> UpdateAsync(string collectorId, CollectorRequest request, CancellationToken cancellationToken = default);

        /// <summary>Permanently deletes a collector.</summary>
        Task DeleteAsync(string collectorId, CancellationToken cancellationToken = default);
    }
}
