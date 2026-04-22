using System;

namespace SurveyMonkeyApi.Client
{
    /// <summary>
    /// Holds the access tokens and throttle settings for each SurveyMonkey resource type.
    /// Each resource type uses its own dedicated access token and throttler.
    /// </summary>
    public class SurveyMonkeyOptions
    {
        /// <summary>
        /// The base URL of the SurveyMonkey API. Defaults to https://api.surveymonkey.com/v3.
        /// </summary>
        public string BaseUrl { get; set; } = "https://api.surveymonkey.com/v3";

        /// <summary>
        /// Access token used for survey operations.
        /// </summary>
        public string SurveyAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token used for collector operations.
        /// </summary>
        public string CollectorAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token used for message operations.
        /// </summary>
        public string MessageAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token used for contact operations.
        /// </summary>
        public string ContactAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Access token used for response operations.
        /// </summary>
        public string ResponseAccessToken { get; set; } = string.Empty;

        /// <summary>
        /// Minimum interval between consecutive requests per resource type.
        /// Defaults to 200 ms (5 requests/second).
        /// </summary>
        public TimeSpan RequestInterval { get; set; } = TimeSpan.FromMilliseconds(200);

        /// <summary>
        /// Maximum number of requests allowed per access token per calendar day (UTC).
        /// Defaults to 500 (SurveyMonkey free-plan limit).
        /// </summary>
        public int DailyRequestLimit { get; set; } = 500;
    }
}
