using System.Text.Json.Serialization;

namespace MovieRecommendationAPI.Models
{
    public class OmdbRating
    {
        [JsonPropertyName("Source")]
        public string Source { get; set; } = string.Empty;

        [JsonPropertyName("Value")]
        public string Value { get; set; } = string.Empty;
    }
}