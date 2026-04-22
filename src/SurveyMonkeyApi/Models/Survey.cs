using System.Text.Json.Serialization;

namespace SurveyMonkeyApi.Models
{
    /// <summary>
    /// Represents a SurveyMonkey survey.
    /// </summary>
    public class Survey
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("folder_id")]
        public string? FolderId { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("question_count")]
        public int QuestionCount { get; set; }

        [JsonPropertyName("page_count")]
        public int PageCount { get; set; }

        [JsonPropertyName("response_count")]
        public int ResponseCount { get; set; }

        [JsonPropertyName("date_created")]
        public string? DateCreated { get; set; }

        [JsonPropertyName("date_modified")]
        public string? DateModified { get; set; }

        [JsonPropertyName("preview")]
        public string? Preview { get; set; }

        [JsonPropertyName("edit_url")]
        public string? EditUrl { get; set; }

        [JsonPropertyName("collect_url")]
        public string? CollectUrl { get; set; }

        [JsonPropertyName("analyze_url")]
        public string? AnalyzeUrl { get; set; }

        [JsonPropertyName("summary_url")]
        public string? SummaryUrl { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }
    }

    /// <summary>
    /// Request body for creating or updating a survey.
    /// </summary>
    public class SurveyRequest
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }

        [JsonPropertyName("language")]
        public string? Language { get; set; }

        [JsonPropertyName("folder_id")]
        public string? FolderId { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }
    }
}
