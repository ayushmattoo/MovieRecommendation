using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MovieRecommendationAPI.Models
{
    public class OmdbMovieDetails
    {
        [JsonPropertyName("Title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("Year")]
        public string Year { get; set; } = string.Empty;

        [JsonPropertyName("Rated")]
        public string Rated { get; set; } = string.Empty;

        [JsonPropertyName("Released")]
        public string Released { get; set; } = string.Empty;

        [JsonPropertyName("Runtime")]
        public string Runtime { get; set; } = string.Empty;

        [JsonPropertyName("Genre")]
        public string Genre { get; set; } = string.Empty;

        [JsonPropertyName("Director")]
        public string Director { get; set; } = string.Empty;

        [JsonPropertyName("Writer")]
        public string Writer { get; set; } = string.Empty;

        [JsonPropertyName("Actors")]
        public string Actors { get; set; } = string.Empty;

        [JsonPropertyName("Plot")]
        public string Plot { get; set; } = string.Empty;

        [JsonPropertyName("Language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("Country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("Awards")]
        public string Awards { get; set; } = string.Empty;

        [JsonPropertyName("Poster")]
        public string Poster { get; set; } = string.Empty;

        [JsonPropertyName("Ratings")]
        public List<OmdbRating> Ratings { get; set; } = new List<OmdbRating>();

        [JsonPropertyName("Metascore")]
        public string Metascore { get; set; } = string.Empty;

        [JsonPropertyName("imdbRating")]
        public string imdbRating { get; set; } = string.Empty;

        [JsonPropertyName("imdbVotes")]
        public string imdbVotes { get; set; } = string.Empty;

        [JsonPropertyName("imdbID")]
        public string imdbID { get; set; } = string.Empty;

        [JsonPropertyName("Type")]
        public string Type { get; set; } = string.Empty;

        [JsonPropertyName("DVD")]
        public string? DVD { get; set; } // Nullable string

        [JsonPropertyName("BoxOffice")]
        public string? BoxOffice { get; set; }

        [JsonPropertyName("Production")]
        public string? Production { get; set; }

        [JsonPropertyName("Website")]
        public string? Website { get; set; }

        [JsonPropertyName("Response")]
        public string Response { get; set; } = string.Empty; // "True" or "False"

        [JsonPropertyName("Error")]
        public string? Error { get; set; } // Error message if Response is "False"

        [JsonIgnore]
        public bool IsSuccessful => "True".Equals(Response, StringComparison.OrdinalIgnoreCase);
    }
}