using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Models;

namespace SurveyMonkeyApi.Services
{
    /// <summary>
    /// CRUD operations for SurveyMonkey collector messages.
    /// </summary>
    public interface IMessageService
    {
        /// <summary>Returns a paged list of messages for the specified collector.</summary>
        Task<PagedResponse<Message>> ListAsync(string collectorId, int page = 1, int perPage = 50, CancellationToken cancellationToken = default);

        /// <summary>Returns details for a single message.</summary>
        Task<Message> GetAsync(string collectorId, string messageId, CancellationToken cancellationToken = default);

        /// <summary>Creates a new message for the specified collector.</summary>
        Task<Message> CreateAsync(string collectorId, MessageRequest request, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing message (partial update).</summary>
        Task<Message> UpdateAsync(string collectorId, string messageId, MessageRequest request, CancellationToken cancellationToken = default);

        /// <summary>Permanently deletes a message.</summary>
        Task DeleteAsync(string collectorId, string messageId, CancellationToken cancellationToken = default);
    }
}
