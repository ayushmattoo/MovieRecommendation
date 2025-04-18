using Microsoft.AspNetCore.Mvc;
using MovieRecommendationAPI.Models;
using MovieRecommendationAPI.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // For logging

namespace MovieRecommendationAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // Base route: /api/movies
    public class MoviesController : ControllerBase
    {
        private readonly OmdbService _omdbService;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(OmdbService omdbService, ILogger<MoviesController> logger)
        {
            _omdbService = omdbService;
            _logger = logger;
        }

        // GET /api/movies/details/tt3896198
        [HttpGet("details/{imdbId}")]
        [ProducesResponseType(typeof(OmdbMovieDetails), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OmdbMovieDetails>> GetMovieDetails(string imdbId)
        {
            _logger.LogInformation("Received request for movie details: {ImdbId}", imdbId);
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return BadRequest(new { Message = "IMDb ID cannot be empty." });
            }

            var movieDetails = await _omdbService.GetMovieDetailsByIdAsync(imdbId);

            if (movieDetails == null)
            {
                // This might indicate a service-level error caught in the service
                _logger.LogError("GetMovieDetailsByIdAsync returned null for {ImdbId}. Potential service error.", imdbId);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while fetching movie details." });
            }

            if (!movieDetails.IsSuccessful)
            {
                _logger.LogWarning("Movie details request failed for {ImdbId}. OMDb Error: {Error}", imdbId, movieDetails.Error);
                return NotFound(new { Message = movieDetails.Error ?? "Movie not found." });
            }

            return Ok(movieDetails);
        }

        // GET /api/movies/search?term=Guardians&page=1&type=movie
        [HttpGet("search")]
        [ProducesResponseType(typeof(OmdbSearchResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)] // If OMDb returns "Movie not found!"
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<OmdbSearchResponse>> SearchMovies([FromQuery] string term, [FromQuery] int page = 1, [FromQuery] string? type = null)
        {
            _logger.LogInformation("Received search request. Term: '{SearchTerm}', Page: {Page}, Type: {Type}", term, page, type ?? "any");
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new { Message = "Search term cannot be empty." });
            }
            if (page < 1) page = 1; // Ensure page is at least 1

            var searchResult = await _omdbService.SearchMoviesAsync(term, page, type);

            if (searchResult == null)
            {
                _logger.LogError("SearchMoviesAsync returned null for term '{SearchTerm}'. Potential service error.", term);
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred during the search." });
            }

            if (!searchResult.IsSuccessful)
            {
                // OMDb often returns "Movie not found!" error for searches with no results, treat as NotFound
                if (searchResult.Error?.Contains("Movie not found", StringComparison.OrdinalIgnoreCase) == true)
                {
                    _logger.LogInformation("Search for '{SearchTerm}' yielded no results from OMDb.", term);
                    // Return an empty successful response structure instead of 404 for search
                    return Ok(new OmdbSearchResponse { Response = "True", Search = new List<OmdbMovieSummary>(), totalResults = "0" });
                    // Or optionally: return NotFound(new { Message = searchResult.Error });
                }
                else
                {
                    _logger.LogWarning("Search request failed for term '{SearchTerm}'. OMDb Error: {Error}", term, searchResult.Error);
                    // For other errors, maybe return 500 or a more specific error if identifiable
                    return StatusCode(StatusCodes.Status500InternalServerError, new { Message = searchResult.Error ?? "Failed to search movies." });
                }
            }

            return Ok(searchResult);
        }

        // GET /api/movies/recommendations/tt3896198
        [HttpGet("recommendations/{imdbId}")]
        [ProducesResponseType(typeof(IEnumerable<OmdbMovieSummary>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<OmdbMovieSummary>>> GetRecommendations(string imdbId)
        {
            _logger.LogInformation("Received recommendation request for base movie: {ImdbId}", imdbId);
            if (string.IsNullOrWhiteSpace(imdbId))
            {
                return BadRequest(new { Message = "IMDb ID cannot be empty for recommendations." });
            }

            var recommendations = await _omdbService.GetRecommendationsAsync(imdbId);

            if (recommendations == null)
            {
                // This indicates an issue getting source movie or extracting genre in the service
                _logger.LogWarning("Could not generate recommendations for {ImdbId}. Source movie/genre likely unavailable.", imdbId);
                return NotFound(new { Message = "Could not generate recommendations. Ensure the source movie ID is valid and has genre information." });
            }

            // Even if the list is empty (no recommendations found for the genre), return 200 OK with the empty list.
            return Ok(recommendations);
        }
    }
}