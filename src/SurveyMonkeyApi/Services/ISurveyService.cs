using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Models;

namespace SurveyMonkeyApi.Services
{
    /// <summary>
    /// CRUD operations for SurveyMonkey surveys.
    /// </summary>
    public interface ISurveyService
    {
        /// <summary>Returns a paged list of surveys owned by the authenticated user.</summary>
        Task<PagedResponse<Survey>> ListAsync(int page = 1, int perPage = 50, CancellationToken cancellationToken = default);

        /// <summary>Returns details for a single survey.</summary>
        Task<Survey> GetAsync(string surveyId, CancellationToken cancellationToken = default);

        /// <summary>Creates a new survey.</summary>
        Task<Survey> CreateAsync(SurveyRequest request, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing survey (partial update).</summary>
        Task<Survey> UpdateAsync(string surveyId, SurveyRequest request, CancellationToken cancellationToken = default);

        /// <summary>Permanently deletes a survey.</summary>
        Task DeleteAsync(string surveyId, CancellationToken cancellationToken = default);
    }
}
