using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieRecommendationAPI.Models
{
    // Represents the overall response structure for a search query (s=...)
    public class OmdbSearchResponse
    {
        [JsonPropertyName("Search")]
        public List<OmdbMovieSummary> Search { get; set; } = new List<OmdbMovieSummary>();

        [JsonPropertyName("totalResults")]
        public string totalResults { get; set; } = "0"; // OMDb returns this as a string

        [JsonPropertyName("Response")]
        public string Response { get; set; } = string.Empty; // "True" or "False"

        [JsonPropertyName("Error")]
        public string? Error { get; set; } // Error message if Response is "False"

        [JsonIgnore]
        public bool IsSuccessful => "True".Equals(Response, StringComparison.OrdinalIgnoreCase);

        [JsonIgnore]
        public int TotalResultsCount => int.TryParse(totalResults, out int count) ? count : 0;
    }
}