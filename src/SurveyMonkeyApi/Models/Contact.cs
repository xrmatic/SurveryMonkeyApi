using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SurveyMonkeyApi.Models
{
    /// <summary>
    /// Represents a SurveyMonkey contact.
    /// </summary>
    public class Contact
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("custom_fields")]
        public Dictionary<string, string>? CustomFields { get; set; }

        [JsonPropertyName("date_created")]
        public string? DateCreated { get; set; }

        [JsonPropertyName("date_modified")]
        public string? DateModified { get; set; }
    }

    /// <summary>
    /// Request body for creating or updating a contact.
    /// </summary>
    public class ContactRequest
    {
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("external_id")]
        public string? ExternalId { get; set; }

        [JsonPropertyName("custom_fields")]
        public Dictionary<string, string>? CustomFields { get; set; }
    }
}
