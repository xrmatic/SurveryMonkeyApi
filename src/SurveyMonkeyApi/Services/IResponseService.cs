using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Models;

namespace SurveyMonkeyApi.Services
{
    /// <summary>
    /// CRUD operations for SurveyMonkey survey responses.
    /// </summary>
    public interface IResponseService
    {
        /// <summary>Returns a paged list of responses for the specified survey.</summary>
        Task<PagedResponse<SurveyResponse>> ListAsync(string surveyId, int page = 1, int perPage = 50, CancellationToken cancellationToken = default);

        /// <summary>Returns a single survey response.</summary>
        Task<SurveyResponse> GetAsync(string surveyId, string responseId, CancellationToken cancellationToken = default);

        /// <summary>Creates a new response for the specified survey.</summary>
        Task<SurveyResponse> CreateAsync(string surveyId, SurveyResponseRequest request, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing response (partial update).</summary>
        Task<SurveyResponse> UpdateAsync(string surveyId, string responseId, SurveyResponseRequest request, CancellationToken cancellationToken = default);

        /// <summary>Permanently deletes a response.</summary>
        Task DeleteAsync(string surveyId, string responseId, CancellationToken cancellationToken = default);
    }
}
