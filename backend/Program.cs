using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Register AppDbContext with PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── JWT Authentication ──────────────────────────────────────────────────────
// The signing key MUST be supplied via the JWT_SECRET_KEY environment variable.
// It must be at least 32 characters long for HMAC-SHA256 security.
// The app will refuse to start if the key is missing or too short.
var jwtKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.StartsWith("PLEASE_SET") || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "STARTUP FAILURE: JWT_SECRET_KEY environment variable is missing, is a placeholder, " +
        "or is shorter than 32 characters. " +
        "Set it via: $env:JWT_SECRET_KEY = 'your-secure-random-key-here' (PowerShell) " +
        "or export JWT_SECRET_KEY='...' (Bash).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });
	
builder.Services.AddAuthorization();

// ── CORS ────────────────────────────────────────────────────────────────────
// In production, set the ALLOWED_ORIGINS environment variable to a
// comma-separated list of allowed frontend origins, e.g.:/
//   ALLOWED_ORIGINS=https://yourdomain.com,https://www.yourdomain.com
// In local development, falls back to the Vite dev server origin.
var rawOrigins = Environment.GetEnvironmentVariable("ALLOWED_ORIGINS");
var allowedOrigins = string.IsNullOrWhiteSpace(rawOrigins)
    ? new[] { "http://localhost:5173" }
    : rawOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ── Rate Limiting ───────────────────────────────────────────────────────────
// "CheckoutLimiter": Fixed Window — max 5 order submissions per client IP
// per 60-second window. Throttled requests receive HTTP 429 Too Many Requests.
// This defends POST /api/orders against bot spam and inventory draining attacks.
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter("CheckoutLimiter", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 5;
        limiterOptions.QueueLimit = 0;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    // Set a Retry-After header so legitimate clients know when to retry.
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.Headers.RetryAfter = "60";
        await context.HttpContext.Response.WriteAsync(
            "{\"message\":\"Too many requests. Please wait before placing another order.\"}",
            cancellationToken);
    };
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// UseRateLimiter must be placed after UseCors and before UseAuthentication/UseAuthorization.
app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Apply migrations and seed data on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();

