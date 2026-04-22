using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SurveyMonkeyApi.Models
{
    /// <summary>
    /// Represents a SurveyMonkey survey response.
    /// </summary>
    public class SurveyResponse
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("survey_id")]
        public string? SurveyId { get; set; }

        [JsonPropertyName("collector_id")]
        public string? CollectorId { get; set; }

        [JsonPropertyName("recipient_id")]
        public string? RecipientId { get; set; }

        [JsonPropertyName("total_time")]
        public int TotalTime { get; set; }

        [JsonPropertyName("custom_value")]
        public string? CustomValue { get; set; }

        [JsonPropertyName("edit_url")]
        public string? EditUrl { get; set; }

        [JsonPropertyName("analyze_url")]
        public string? AnalyzeUrl { get; set; }

        [JsonPropertyName("ip_address")]
        public string? IpAddress { get; set; }

        [JsonPropertyName("custom_variables")]
        public Dictionary<string, string>? CustomVariables { get; set; }

        [JsonPropertyName("response_status")]
        public string? ResponseStatus { get; set; }

        [JsonPropertyName("collection_mode")]
        public string? CollectionMode { get; set; }

        [JsonPropertyName("date_created")]
        public string? DateCreated { get; set; }

        [JsonPropertyName("date_modified")]
        public string? DateModified { get; set; }

        [JsonPropertyName("pages")]
        public List<ResponsePage>? Pages { get; set; }
    }

    /// <summary>
    /// Represents a page within a survey response.
    /// </summary>
    public class ResponsePage
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("questions")]
        public List<ResponseQuestion>? Questions { get; set; }
    }

    /// <summary>
    /// Represents a question answer within a survey response page.
    /// </summary>
    public class ResponseQuestion
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("variable_id")]
        public string? VariableId { get; set; }

        [JsonPropertyName("answers")]
        public List<ResponseAnswer>? Answers { get; set; }
    }

    /// <summary>
    /// Represents an answer to a question in a survey response.
    /// </summary>
    public class ResponseAnswer
    {
        [JsonPropertyName("choice_id")]
        public string? ChoiceId { get; set; }

        [JsonPropertyName("row_id")]
        public string? RowId { get; set; }

        [JsonPropertyName("col_id")]
        public string? ColId { get; set; }

        [JsonPropertyName("other_id")]
        public string? OtherId { get; set; }

        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    /// <summary>
    /// Request body for creating or updating a survey response.
    /// </summary>
    public class SurveyResponseRequest
    {
        [JsonPropertyName("custom_value")]
        public string? CustomValue { get; set; }

        [JsonPropertyName("custom_variables")]
        public Dictionary<string, string>? CustomVariables { get; set; }

        [JsonPropertyName("pages")]
        public List<ResponsePage>? Pages { get; set; }
    }
}
