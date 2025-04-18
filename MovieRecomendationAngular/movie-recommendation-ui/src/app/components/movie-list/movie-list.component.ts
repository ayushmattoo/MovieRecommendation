// --- File: src/app/components/movie-list/movie-list.component.ts ---
import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common'; // <-- For *ngIf, *ngFor
import { RouterModule } from '@angular/router'; // <-- For [routerLink]
import { OmdbMovieSummary } from '../../models/omdb.model'; // Correct path?

@Component({
  selector: 'app-movie-list',
  standalone: true,
  imports: [CommonModule, RouterModule], // <-- Ensure these are imported
  templateUrl: './movie-list.component.html',
  styleUrls: ['./movie-list.component.css']
})
export class MovieListComponent {
  // Input property to receive the array of movies from the parent component
  @Input() movies: OmdbMovieSummary[] | null = []; // Allow null input
  // Input property for an optional title above the list
  @Input() listTitle: string = '';

  // Function to handle posters that are 'N/A' or missing
  getPosterUrl(url: string | undefined): string {
    // Provide a path to a default placeholder image in your assets folder
    const defaultPoster = 'assets/images/default-poster.png';
    return (url && url !== 'N/A') ? url : defaultPoster;
  }
}