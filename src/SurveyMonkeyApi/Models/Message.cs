using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SurveyMonkeyApi.Models
{
    /// <summary>
    /// Represents a SurveyMonkey collector message (invitation or reminder).
    /// </summary>
    public class Message
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("href")]
        public string? Href { get; set; }

        [JsonPropertyName("is_branding_enabled")]
        public bool IsBrandingEnabled { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("body_text")]
        public string? BodyText { get; set; }

        [JsonPropertyName("body_html")]
        public string? BodyHtml { get; set; }

        [JsonPropertyName("date_created")]
        public string? DateCreated { get; set; }

        [JsonPropertyName("date_modified")]
        public string? DateModified { get; set; }

        [JsonPropertyName("recipients")]
        public List<string>? Recipients { get; set; }

        [JsonPropertyName("recipient_status")]
        public string? RecipientStatus { get; set; }
    }

    /// <summary>
    /// Request body for creating or updating a message.
    /// </summary>
    public class MessageRequest
    {
        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("subject")]
        public string? Subject { get; set; }

        [JsonPropertyName("body_text")]
        public string? BodyText { get; set; }

        [JsonPropertyName("body_html")]
        public string? BodyHtml { get; set; }

        [JsonPropertyName("is_branding_enabled")]
        public bool? IsBrandingEnabled { get; set; }

        [JsonPropertyName("recipient_status")]
        public string? RecipientStatus { get; set; }
    }
}
