using System.Text.Json.Serialization;

namespace SurveyMonkeyApi.Models
{
    /// <summary>
    /// Represents a SurveyMonkey collector (distribution channel).
    /// </summary>
    public class Collector
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("thank_you_message")]
        public string? ThankYouMessage { get; set; }

        [JsonPropertyName("disqualification_message")]
        public string? DisqualificationMessage { get; set; }

        [JsonPropertyName("close_date")]
        public string? CloseDate { get; set; }

        [JsonPropertyName("closed_page_message")]
        public string? ClosedPageMessage { get; set; }

        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; set; }

        [JsonPropertyName("display_survey_results")]
        public bool DisplaySurveyResults { get; set; }

        [JsonPropertyName("edit_response_type")]
        public string? EditResponseType { get; set; }

        [JsonPropertyName("anonymous_type")]
        public string? AnonymousType { get; set; }

        [JsonPropertyName("allow_multiple_responses")]
        public bool AllowMultipleResponses { get; set; }

        [JsonPropertyName("date_created")]
        public string? DateCreated { get; set; }

        [JsonPropertyName("date_modified")]
        public string? DateModified { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }
    }

    /// <summary>
    /// Request body for creating or updating a collector.
    /// </summary>
    public class CollectorRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("thank_you_message")]
        public string? ThankYouMessage { get; set; }

        [JsonPropertyName("disqualification_message")]
        public string? DisqualificationMessage { get; set; }

        [JsonPropertyName("close_date")]
        public string? CloseDate { get; set; }

        [JsonPropertyName("closed_page_message")]
        public string? ClosedPageMessage { get; set; }

        [JsonPropertyName("redirect_url")]
        public string? RedirectUrl { get; set; }

        [JsonPropertyName("display_survey_results")]
        public bool? DisplaySurveyResults { get; set; }

        [JsonPropertyName("allow_multiple_responses")]
        public bool? AllowMultipleResponses { get; set; }

        [JsonPropertyName("anonymous_type")]
        public string? AnonymousType { get; set; }

        [JsonPropertyName("edit_response_type")]
        public string? EditResponseType { get; set; }
    }
}
