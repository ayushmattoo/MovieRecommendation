// --- File: src/app/models/omdb.model.ts ---

// Represents a single rating source and value
export interface OmdbRating {
    Source: string;
    Value: string;
  }
  
  // Represents a single movie entry in the search results
  export interface OmdbMovieSummary {
    Title: string;
    Year: string;
    imdbID: string;
    Type: string; // e.g., "movie", "series"
    Poster: string; // URL or "N/A"
  }
  
  // Represents the overall response structure for a search query (s=...)
  // Matches the backend's OmdbSearchResponse
  export interface OmdbSearchResponse {
    Search: OmdbMovieSummary[];
    totalResults: string; // OMDb returns this as a string
    Response: string; // "True" or "False"
    Error?: string; // Error message if Response is "False"
  }
  
  
  // Represents the main structure of the movie details response
  // Matches the backend's OmdbMovieDetails
  export interface OmdbMovieDetails {
    Title: string;
    Year: string;
    Rated: string;
    Released: string;
    Runtime: string;
    Genre: string; // Comma-separated string
    Director: string;
    Writer: string;
    Actors: string; // Comma-separated string
    Plot: string;
    Language: string;
    Country: string;
    Awards: string;
    Poster: string; // URL string
    Ratings: OmdbRating[]; // Array of rating objects
    Metascore: string;
    imdbRating: string;
    imdbVotes: string;
    imdbID: string;
    Type: string;
    DVD?: string;
    BoxOffice?: string;
    Production?: string;
    Website?: string;
    Response: string; // "True" or "False"
    Error?: string;
  }
  
  // Optional: Interface for the backend error response structure
  export interface ApiError {
    Message: string;
  }