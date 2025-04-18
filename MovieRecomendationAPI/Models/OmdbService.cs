using System.Text.Json.Serialization;

namespace MovieRecommendationAPI.Models
{
    // Represents a single movie entry in the search results
    public class OmdbMovieSummary
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("Year")]
        public string Year { get; set; } = string.Empty;

        [JsonPropertyName("imdbID")]
        public string imdbID { get; set; } = string.Empty;

        [JsonPropertyName("Type")]
        public string Type { get; set; } = string.Empty; // e.g., "movie", "series"

        [JsonPropertyName("Poster")]
        public string Poster { get; set; } = string.Empty; // URL or "N/A"
    }
}