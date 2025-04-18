// --- File: src/app/app.config.ts ---
import { ApplicationConfig, importProvidersFrom } from '@angular/core';
import { provideRouter } from '@angular/router'; // Likely already here
import { provideHttpClient } from '@angular/common/http'; // <-- Import this

import { routes } from './app.routes'; // Likely already here

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes), // Keep this if present
    provideHttpClient()    // <-- Add this provider
    // Add other providers here if needed
  ]
};