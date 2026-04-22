using System.Threading;
using System.Threading.Tasks;
using SurveyMonkeyApi.Models;

namespace SurveyMonkeyApi.Services
{
    /// <summary>
    /// CRUD operations for SurveyMonkey contacts.
    /// </summary>
    public interface IContactService
    {
        /// <summary>Returns a paged list of contacts in the contact list.</summary>
        Task<PagedResponse<Contact>> ListAsync(int page = 1, int perPage = 50, CancellationToken cancellationToken = default);

        /// <summary>Returns details for a single contact.</summary>
        Task<Contact> GetAsync(string contactId, CancellationToken cancellationToken = default);

        /// <summary>Creates a new contact.</summary>
        Task<Contact> CreateAsync(ContactRequest request, CancellationToken cancellationToken = default);

        /// <summary>Updates an existing contact (partial update).</summary>
        Task<Contact> UpdateAsync(string contactId, ContactRequest request, CancellationToken cancellationToken = default);

        /// <summary>Permanently deletes a contact.</summary>
        Task DeleteAsync(string contactId, CancellationToken cancellationToken = default);
    }
}
