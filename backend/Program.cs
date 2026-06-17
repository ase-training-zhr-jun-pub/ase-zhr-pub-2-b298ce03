using Calvin.BookingService.Data;
using Calvin.BookingService.Endpoints;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryStore>();

// Behind the Crucible proxy, frontend and backend share the same origin
// (crucible.ch.innoq.io), so the browser sends no CORS preflight.
// For local dev, frontend (5173) and backend (5000) are cross-origin.
var allowedOrigins = builder.Configuration["Cors:AllowedOrigins"]
    ?.Split(',', StringSplitOptions.RemoveEmptyEntries)
    ?? ["http://localhost:5173"];

// If running behind the Crucible proxy, also allow that origin.
var proxyUri = builder.Configuration["VSCODE_PROXY_URI"]
    ?? Environment.GetEnvironmentVariable("VSCODE_PROXY_URI");
if (proxyUri is not null)
{
    var proxyOrigin = new Uri(proxyUri.Replace("{{port}}", "5173")).GetLeftPart(UriPartial.Authority);
    allowedOrigins = [..allowedOrigins, proxyOrigin];
}

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()));

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseCors();
app.MapOpenApi();
app.MapScalarApiReference();

app.MapLocations();
app.MapConferenceRooms();
app.MapWorkplaces();
app.MapBookings();
app.MapUsers(); // endpoints registered

app.Run();
