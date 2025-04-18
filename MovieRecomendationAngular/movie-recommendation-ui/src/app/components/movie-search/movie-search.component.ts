// --- File: src/app/components/movie-search/movie-search.component.ts ---
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms'; // <-- Needed for ngModel

@Component({
  selector: 'app-movie-search',
  standalone: true,
  imports: [FormsModule], // <-- Import FormsModule
  templateUrl: './movie-search.component.html',
  styleUrls: ['./movie-search.component.css']
})
export class MovieSearchComponent {
  searchTerm: string = '';

  constructor(private router: Router) { }

  // Method called when the search button is clicked or Enter is pressed
  onSearch(): void {
    const termToSearch = this.searchTerm.trim();
    if (termToSearch) {
      // Navigate to the SearchResultsComponent route, passing the term as a query parameter
      this.router.navigate(['/search'], { queryParams: { term: termToSearch } });
      // Optionally clear the search bar after navigating
      // this.searchTerm = '';
    }
  }
}