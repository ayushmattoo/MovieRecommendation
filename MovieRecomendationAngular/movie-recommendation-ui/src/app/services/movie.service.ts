// --- File: src/app/services/movie.service.ts ---
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs'; // Make sure Observable/throwError are imported
import { catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment'; // Assuming you created this
// If no environments folder, replace previous line and next line with:
// private apiUrl = 'YOUR_API_URL/api/movies'; // Hardcoded URL
import {
  OmdbSearchResponse,
  OmdbMovieDetails,
  OmdbMovieSummary,
  ApiError
} from '../models/omdb.model';

@Injectable({
  providedIn: 'root'
})
export class MovieService {
  // Use environment variable or hardcoded URL
  private apiUrl = `${environment.apiUrl}/movies`;

  constructor(private http: HttpClient) { }

  // --- VERIFY/ADD THIS METHOD ---
  searchMovies(term: string, page: number = 1, type?: string): Observable<OmdbSearchResponse> {
    if (!term.trim()) {
       return new Observable<OmdbSearchResponse>(observer => {
          observer.next({ Search: [], totalResults: "0", Response: "True" });
          observer.complete();
       });
    }

    let params = new HttpParams()
      .set('term', term)
      .set('page', page.toString());

    if (type && (type === 'movie' || type === 'series' || type === 'episode')) {
      params = params.set('type', type);
    }

    return this.http.get<OmdbSearchResponse>(`${this.apiUrl}/search`, { params })
      .pipe(
        catchError(this.handleError)
      );
  }

  // Method to get movie details (Ensure this one exists too)
  getMovieDetails(imdbId: string): Observable<OmdbMovieDetails> {
    if (!imdbId) {
       return throwError(() => new Error('IMDb ID is required.'));
    }
    return this.http.get<OmdbMovieDetails>(`${this.apiUrl}/details/${imdbId}`)
      .pipe(
        catchError(this.handleError)
      );
  }

  // --- VERIFY/ADD THIS METHOD ---
  getRecommendations(imdbId: string): Observable<OmdbMovieSummary[]> {
     if (!imdbId) {
       return throwError(() => new Error('IMDb ID is required for recommendations.'));
     }
    return this.http.get<OmdbMovieSummary[]>(`${this.apiUrl}/recommendations/${imdbId}`)
      .pipe(
        catchError(this.handleError)
      );
  }


  // Centralized Error Handling (Ensure this exists)
  private handleError(error: HttpErrorResponse) {
    let userMessage = 'An unknown error occurred!';
    console.error('API Error:', error);
    // ... (rest of error handling logic from previous step) ...
     if (error.error instanceof ErrorEvent) {
       userMessage = `Network error: ${error.error.message}`;
     } else {
       userMessage = `Server error: ${error.status}`;
       if (error.error && typeof error.error === 'object') {
          const apiError = error.error as ApiError;
          if (apiError.Message) { userMessage = apiError.Message; }
          else if (error.error.Error) { userMessage = error.error.Error; }
       } else if (typeof error.error === 'string') { userMessage = error.error; }
     }
    return throwError(() => new Error(userMessage));
  }
}