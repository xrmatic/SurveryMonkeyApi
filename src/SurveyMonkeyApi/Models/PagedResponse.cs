using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SurveyMonkeyApi.Models
{
    /// <summary>
    /// Represents a paged list response from the SurveyMonkey API.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PagedResponse<T>
    {
        [JsonPropertyName("data")]
        public List<T> Data { get; set; } = new();

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("links")]
        public PageLinks? Links { get; set; }
    }

    /// <summary>
    /// Contains navigation links for a paged API response.
    /// </summary>
    public class PageLinks
    {
        [JsonPropertyName("self")]
        public string? Self { get; set; }

        [JsonPropertyName("prev")]
        public string? Prev { get; set; }

        [JsonPropertyName("next")]
        public string? Next { get; set; }

        [JsonPropertyName("first")]
        public string? First { get; set; }

        [JsonPropertyName("last")]
        public string? Last { get; set; }
    }
}
