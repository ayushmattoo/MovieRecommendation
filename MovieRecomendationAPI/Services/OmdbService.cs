using Microsoft.Extensions.Options;
using MovieRecommendationAPI.Models;
using System.Net.Http;
using System.Net.Http.Json; // For GetFromJsonAsync
using System.Threading.Tasks;
using System.Web; // For HttpUtility needed for UrlEncode
using Microsoft.Extensions.Logging; // For logging
using System;
using System.Linq;
using System.Collections.Generic; // For IEnumerable


namespace MovieRecommendationAPI.Services
{
    public class OmdbService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OmdbService> _logger;

        // Option 1: Inject HttpClient directly (if configured as typed client)
        public OmdbService(HttpClient httpClient, IConfiguration configuration, ILogger<OmdbService> logger)
        {
            _httpClient = httpClient;
            // Safer way to get config, ensure it exists
            _apiKey = configuration["Omdb:ApiKey"] ?? throw new InvalidOperationException("OMDb API Key not found in configuration.");
            _logger = logger;

            if (string.IsNullOrWhiteSpace(configuration["Omdb:BaseUrl"]))
            {
                throw new InvalidOperationException("OMDb BaseUrl not found in configuration.");
            }
            // BaseAddress should be set in Program.cs when configuring the typed client
            // If not using typed client, set it here:
            // _httpClient.BaseAddress = new Uri(configuration["Omdb:BaseUrl"]);
        }

        // --- Method to get movie details by IMDb ID ---
        public async Task<OmdbMovieDetails?> GetMovieDetailsByIdAsync(string imdbId)
        {
            if (string.IsNullOrWhiteSpace(imdbId)) return null;

            var requestUri = $"?i={Uri.EscapeDataString(imdbId)}&apikey={_apiKey}&plot=full"; // Request full plot

            try
            {
                _logger.LogInformation("Requesting movie details from OMDb for ID: {ImdbId}", imdbId);
                var response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var movieDetails = await response.Content.ReadFromJsonAsync<OmdbMovieDetails>();
                    _logger.LogInformation("Successfully fetched details for {ImdbId}. Response: {IsSuccessful}", imdbId, movieDetails?.IsSuccessful);
                    return movieDetails;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("OMDb API returned non-success status {StatusCode} for ID {ImdbId}. Content: {ErrorContent}", (int)response.StatusCode, imdbId, errorContent);
                    // Try to deserialize error response if possible, otherwise return generic failure
                    try
                    {
                        var errorDetails = System.Text.Json.JsonSerializer.Deserialize<OmdbMovieDetails>(errorContent);
                        if (errorDetails != null && !errorDetails.IsSuccessful) return errorDetails; // Return the error details from OMDb
                    }
                    catch (Exception jsonEx)
                    {
                        _logger.LogError(jsonEx, "Failed to deserialize OMDb error response for ID {ImdbId}", imdbId);
                    }
                    return new OmdbMovieDetails { Response = "False", Error = $"API request failed with status code {response.StatusCode}" };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error while fetching details for ID {ImdbId}", imdbId);
                return new OmdbMovieDetails { Response = "False", Error = "Network error occurred while contacting OMDb." };
            }
            catch (Exception ex) // Catch other potential exceptions (e.g., JsonException)
            {
                _logger.LogError(ex, "Error fetching movie details for ID {ImdbId}", imdbId);
                return new OmdbMovieDetails { Response = "False", Error = "An unexpected error occurred." };
            }
        }

        // --- Method to search movies by title ---
        public async Task<OmdbSearchResponse?> SearchMoviesAsync(string searchTerm, int page = 1, string? type = null)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return null;

            var encodedSearchTerm = Uri.EscapeDataString(searchTerm);
            var requestUri = $"?s={encodedSearchTerm}&page={page}&apikey={_apiKey}";
            if (!string.IsNullOrWhiteSpace(type) && (type == "movie" || type == "series" || type == "episode"))
            {
                requestUri += $"&type={type}";
            }


            try
            {
                _logger.LogInformation("Searching OMDb for term: '{SearchTerm}', page: {Page}, type: {Type}", searchTerm, page, type ?? "any");
                var response = await _httpClient.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var searchResponse = await response.Content.ReadFromJsonAsync<OmdbSearchResponse>();
                    _logger.LogInformation("Successfully searched for '{SearchTerm}'. Response: {IsSuccessful}, Results: {Count}", searchTerm, searchResponse?.IsSuccessful, searchResponse?.Search?.Count ?? 0);
                    return searchResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("OMDb API returned non-success status {StatusCode} for search '{SearchTerm}'. Content: {ErrorContent}", (int)response.StatusCode, searchTerm, errorContent);
                    // Try to deserialize error response
                    try
                    {
                        var errorResponse = System.Text.Json.JsonSerializer.Deserialize<OmdbSearchResponse>(errorContent);
                        if (errorResponse != null && !errorResponse.IsSuccessful) return errorResponse;
                    }
                    catch (Exception jsonEx)
                    {
                        _logger.LogError(jsonEx, "Failed to deserialize OMDb error response for search '{SearchTerm}'", searchTerm);
                    }
                    return new OmdbSearchResponse { Response = "False", Error = $"API request failed with status code {response.StatusCode}" };
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP request error during search for '{SearchTerm}'", searchTerm);
                return new OmdbSearchResponse { Response = "False", Error = "Network error occurred while contacting OMDb." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching movies for '{SearchTerm}'", searchTerm);
                return new OmdbSearchResponse { Response = "False", Error = "An unexpected error occurred." };
            }
        }

        // --- Method to get recommendations (Simple Genre-Based) ---
        public async Task<IEnumerable<OmdbMovieSummary>?> GetRecommendationsAsync(string imdbId, int maxRecommendations = 5)
        {
            _logger.LogInformation("Generating recommendations based on movie ID: {ImdbId}", imdbId);

            // 1. Get details of the source movie
            var sourceMovieDetails = await GetMovieDetailsByIdAsync(imdbId);

            if (sourceMovieDetails == null || !sourceMovieDetails.IsSuccessful || string.IsNullOrWhiteSpace(sourceMovieDetails.Genre))
            {
                _logger.LogWarning("Cannot generate recommendations. Source movie details not found or genre is missing for {ImdbId}.", imdbId);
                return null; // Cannot recommend if source movie details or genre are missing
            }

            // 2. Extract the primary genre (can be more sophisticated later)
            var primaryGenre = sourceMovieDetails.Genre.Split(',').FirstOrDefault()?.Trim();

            if (string.IsNullOrWhiteSpace(primaryGenre))
            {
                _logger.LogWarning("Cannot generate recommendations. Failed to extract primary genre for {ImdbId}.", imdbId);
                return null;
            }

            _logger.LogInformation("Using primary genre '{PrimaryGenre}' for recommendations based on {ImdbId}", primaryGenre, imdbId);

            // 3. Search for movies of the same primary genre (only movies for simplicity)
            var searchResponse = await SearchMoviesAsync(primaryGenre, 1, "movie"); // Search page 1 for movies of that genre

            if (searchResponse == null || !searchResponse.IsSuccessful || searchResponse.Search == null || !searchResponse.Search.Any())
            {
                _logger.LogWarning("No recommendations found for genre '{PrimaryGenre}' based on movie {ImdbId}.", primaryGenre, imdbId);
                return Enumerable.Empty<OmdbMovieSummary>(); // Return empty list if search fails or yields no results
            }

            // 4. Filter results: remove original movie, take top N
            var recommendations = searchResponse.Search
                .Where(movie => movie.imdbID != imdbId) // Exclude the original movie
                .Take(maxRecommendations)               // Limit the number of results
                .ToList();

            _logger.LogInformation("Found {RecommendationCount} recommendations for {ImdbId} based on genre '{PrimaryGenre}'.", recommendations.Count, imdbId, primaryGenre);
            return recommendations;
        }
    }
}