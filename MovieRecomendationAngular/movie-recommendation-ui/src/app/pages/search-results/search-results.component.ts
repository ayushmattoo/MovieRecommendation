// --- File: src/app/pages/search-results/search-results.component.ts ---
import { Observable, Subscription, Observer } from 'rxjs';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MovieService } from '../../services/movie.service'; // Correct path?
import { OmdbSearchResponse, OmdbMovieSummary } from '../../models/omdb.model'; // Correct path?
import { switchMap } from 'rxjs/operators';
import { CommonModule } from '@angular/common'; // <-- For *ngIf etc.
import { MovieListComponent } from '../../components/movie-list/movie-list.component'; // <-- Import list component

@Component({
  selector: 'app-search-results',
  standalone: true,
  imports: [CommonModule, MovieListComponent], // <-- Import CommonModule and MovieListComponent
  templateUrl: './search-results.component.html',
  styleUrls: ['./search-results.component.css']
})
export class SearchResultsComponent implements OnInit, OnDestroy {
  searchResults: OmdbMovieSummary[] = [];
  isLoading: boolean = false;
  error: string | null = null;
  searchTerm: string = '';
  totalResults: number = 0;
  private querySub: Subscription | null = null;

  constructor(
    private route: ActivatedRoute,
    private movieService: MovieService
  ) { }

  ngOnInit(): void {
    this.querySub = this.route.queryParamMap.pipe(
      switchMap(params => {
        this.searchTerm = params.get('term') || '';
        this.isLoading = true;
        this.error = null;
        this.searchResults = []; // Clear previous results
        this.totalResults = 0;
        // Only search if term is not empty
        if (this.searchTerm) {
           return this.movieService.searchMovies(this.searchTerm, 1); // Fetch page 1 initially
        } else {
           // If no search term, immediately return an empty result structure
           return new Observable<OmdbSearchResponse>(observer => {
              observer.next({ Search: [], totalResults: "0", Response: "True" });
              observer.complete();
           });
        }
      })
    ).subscribe({
      next: (response: OmdbSearchResponse) => {
        if (response && response.Response === 'True') {
          this.searchResults = response.Search || [];
          this.totalResults = parseInt(response.totalResults || '0', 10);
        } else {
          // Handle "Movie not found!" specifically - not really an error, just no results
          if (response.Error?.includes('Movie not found')) {
             this.error = null; // Clear any previous error
             this.searchResults = [];
             this.totalResults = 0;
          } else {
             this.error = response.Error || 'An error occurred during the search.';
             this.searchResults = [];
             this.totalResults = 0;
          }
        }
        this.isLoading = false;
      },
      error: (err: Error) => {
        this.error = err.message;
        this.isLoading = false;
        this.searchResults = [];
        this.totalResults = 0;
        console.error("Search error:", err);
      }
    });
  }

  ngOnDestroy(): void {
    this.querySub?.unsubscribe(); // Clean up subscription
  }
}