// --- File: src/app/app.routes.ts ---
import { Routes } from '@angular/router';
import { MovieDetailsComponent } from './pages/movie-details/movie-details.component';
import { SearchResultsComponent } from './pages/search-results/search-results.component';
// Potentially import a HomeComponent if you create one

export const routes: Routes = [
    // Redirect empty path to search (or a future home page)
    { path: '', redirectTo: '/search', pathMatch: 'full' },

    // Route for displaying search results
    // Use component directly (no need for loadChildren with standalone)
    { path: 'search', component: SearchResultsComponent },

    // Route for displaying movie details
    { path: 'movie/:id', component: MovieDetailsComponent },

    // Catch-all (optional)
    { path: '**', redirectTo: '/search' }
    // { path: 'home', component: HomeComponent }, // Example if you add a home page
];