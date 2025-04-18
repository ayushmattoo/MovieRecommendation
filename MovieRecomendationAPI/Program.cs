using MovieRecommendationAPI.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.Configuration; // Required for IConfiguration

var builder = WebApplication.CreateBuilder(args);

// --- Configure Services ---

// 1. Add Controllers Service
builder.Services.AddControllers();

// 2. Add Logging
builder.Services.AddLogging();

// 3. Configure CORS (Cross-Origin Resource Sharing)
var angularAppUrl = builder.Configuration.GetValue<string>("AngularAppUrl") ?? "http://localhost:4200"; // Default Angular dev URL
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            policy.WithOrigins(angularAppUrl) // URL of your Angular app
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// 4. Configure Typed HttpClient for OmdbService
builder.Services.AddHttpClient<OmdbService>((serviceProvider, client) =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    var baseUrl = configuration["Omdb:BaseUrl"];
    if (string.IsNullOrWhiteSpace(baseUrl))
    {
        throw new InvalidOperationException("OMDb BaseUrl is not configured in appsettings.json");
    }
    client.BaseAddress = new Uri(baseUrl);
    
});
// OmdbService itself is automatically registered as transient because AddHttpClient<TClient> does it.


// 5. Add Swagger/OpenAPI (Optional but recommended for API testing)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Movie Recommendation API", Version = "v1" });
});


// --- Build the App ---
var app = builder.Build();

// --- Configure the HTTP request pipeline ---

// Configure Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Recommendation API v1"));
    app.UseDeveloperExceptionPage(); // More detailed errors in dev
}
else
{
    // Add production error handling, HSTS, etc. here
    app.UseExceptionHandler("/error"); // Example: Need to create an error endpoint
    app.UseHsts();
}

app.UseHttpsRedirection(); // Redirect HTTP to HTTPS

app.UseRouting(); // Must be before UseCors and UseAuthorization

app.UseCors("AllowAngularApp"); // Apply the CORS policy

app.UseAuthorization(); // If you add authentication/authorization later

app.MapControllers(); // Map controller routes

// --- Run the App ---
app.Run();