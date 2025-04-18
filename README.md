Movie Recommendation Web App (using ASP.NET Core Web API, Angular, OMDb API)
Developed a full-stack web application enabling users to search for movies, view detailed information, and receive basic genre-based recommendations, utilizing an ASP.NET Core Web API backend and an Angular (Standalone Components) frontend.
Engineered RESTful API endpoints within the .NET Core backend to interface with the frontend, handling requests for movie searches (by title/ID from OMDb) and generating simple content-based recommendations.
Integrated the external OMDb REST API within a dedicated .NET service layer, managing HTTP requests (HttpClientFactory), secure API key usage (via Configuration), and JSON data deserialization into C# models.
Implemented Cross-Origin Resource Sharing (CORS) policies in the ASP.NET Core application pipeline to allow secure communication with the Angular frontend origin.
Utilized Dependency Injection in .NET to manage service lifetimes (e.g., OmdbService, HttpClient).
Consumed the custom .NET Web API from the Angular frontend using TypeScript services (HttpClient), components, and routing to display movie data and recommendations dynamically to the user.
