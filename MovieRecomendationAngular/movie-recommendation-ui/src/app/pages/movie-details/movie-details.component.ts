// --- File: src/app/pages/movie-details/movie-details.component.ts ---
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router'; // Import RouterModule
import { MovieService } from '../../services/movie.service'; // Correct path?
import { OmdbMovieDetails, OmdbMovieSummary } from '../../models/omdb.model'; // Correct path?
import { Subscription, forkJoin, of } from 'rxjs';
import { switchMap, catchError } from 'rxjs/operators';
import { CommonModule } from '@angular/common'; // <-- For *ngIf etc.
import { MovieListComponent } from '../../components/movie-list/movie-list.component'; // <-- Import list component

@Component({
  selector: 'app-movie-details',
  standalone: true,
  // Import CommonModule, MovieListComponent, RouterModule (for potential back links etc.)
  imports: [CommonModule, MovieListComponent, RouterModule],
  templateUrl: './movie-details.component.html',
  styleUrls: ['./movie-details.component.css']
})
export class MovieDetailsComponent implements OnInit, OnDestroy {
  movie: OmdbMovieDetails | null = null;
  recommendations: OmdbMovieSummary[] = [];
  isLoading: boolean = false;
  error: string | null = null;
  private routeSub: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService,
    private router: Router
  ) { }

  ngOnInit(): void {
    this.routeSub = this.route.paramMap.pipe(
      switchMap(params => {
        const imdbId = params.get('id');
        if (!imdbId) {
          // If no ID, redirect to search or show error immediately
          this.error = 'No Movie ID provided in the URL.';
          this.isLoading = false;
          this.router.navigate(['/search']); // Redirect to search page
          return of(null); // Stop the observable chain
        }

        this.isLoading = true;
        this.error = null;
        this.movie = null;
        this.recommendations = [];

        // Use forkJoin to fetch details and recommendations in parallel
        return forkJoin({
          details: this.movieService.getMovieDetails(imdbId).pipe(
             // Handle errors for details separately if needed
             catchError(detailsError => {
                console.error("Error fetching details:", detailsError);
                // Return an observable containing an error state or null
                return of({ Response: 'False', Error: 'Failed to load movie details.' } as OmdbMovieDetails);
             })
          ),
          recs: this.movieService.getRecommendations(imdbId).pipe(
             // Handle errors for recommendations separately
             catchError(recsError => {
                console.error("Error fetching recommendations:", recsError);
                return of([] as OmdbMovieSummary[]); // Return empty array on error
             })
          )
        });
      })
    ).subscribe({
      next: (results) => {
         if (!results) return; // Exit if ID was missing

        // Process Details
        if (results.details && results.details.Response === 'True') {
          this.movie = results.details;
        } else {
          // Set error based on details response, even if recs succeed
          this.error = results.details?.Error || 'Movie details could not be loaded.';
          this.movie = null;
        }

        // Process Recommendations (independent of details success)
        this.recommendations = results.recs || [];

        this.isLoading = false;
      },
      error: (err: Error) => {
         // This catch is for fundamental errors in forkJoin or switchMap upstream
        console.error("Critical error fetching movie data:", err);
        this.error = 'An unexpected error occurred loading movie data. Please try again later.';
        this.isLoading = false;
        this.movie = null;
        this.recommendations = [];
      }
    });
  }

  ngOnDestroy(): void {
    this.routeSub?.unsubscribe();
  }

  // Helper function for default poster
  getPosterUrl(url: string | undefined): string {
    const defaultPoster = 'assets/images/default-poster.png';
    return (url && url !== 'N/A') ? url : defaultPoster;
  }
}