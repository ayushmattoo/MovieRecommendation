// --- File: src/app/app.component.ts ---

import { Component } from '@angular/core';
import { RouterModule } from '@angular/router'; // <-- Import RouterModule for router-outlet
import { MovieSearchComponent } from './components/movie-search/movie-search.component'; // <-- Import MovieSearchComponent

@Component({
  selector: 'app-root',
  standalone: true, // <-- Verify this is true
  imports: [
    RouterModule,          // <-- Add RouterModule here
    MovieSearchComponent   // <-- Add MovieSearchComponent here
  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'movie-recommendation-ui'; // Or whatever your title is
}