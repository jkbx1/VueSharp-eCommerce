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
// Key resolution order (most to least specific):
//   1. builder.Configuration["JWT_SECRET_KEY"]  ← .NET User Secrets (local dev)
//   2. Environment.GetEnvironmentVariable(...)   ← OS env / Render secret
//   3. builder.Configuration["Jwt:Key"]          ← appsettings fallback (never a real secret)
// The key must be at least 32 characters long for HMAC-SHA256 security.
// The app will refuse to start if the key is missing or too short.
var jwtKey = builder.Configuration["JWT_SECRET_KEY"]
    ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY")
    ?? builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey) || jwtKey.StartsWith("PLEASE_SET") || jwtKey.Length < 32)
{
    throw new InvalidOperationException(
        "STARTUP FAILURE: JWT_SECRET_KEY is missing, is a placeholder, " +
        "or is shorter than 32 characters. " +
        "Local dev  → dotnet user-secrets set \"JWT_SECRET_KEY\" \"your-key\" " +
        "Production → set the JWT_SECRET_KEY secret on Render.");
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

// Apply migrations and seed data on startup.
// In Development the migration is best-effort: if the database is unreachable
// (e.g. Supabase pooler is sleeping or local internet is flaky) we log the
// error and continue so the API can still serve non-DB routes.
// In Production the exception propagates and prevents a half-broken deployment.
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider
        .GetRequiredService<ILoggerFactory>()
        .CreateLogger("Startup");

    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (app.Environment.IsDevelopment())
    {
        try
        {
            db.Database.Migrate();
            logger.LogInformation("Database migration applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogWarning(
                ex,
                "[DEV] Database migration failed — the API will start anyway. " +
                "Check your connection string or Supabase pooler status.");
        }
    }
    else
    {
        // Production: let any migration failure surface immediately so the
        // deployment is rolled back rather than running with a stale schema.
        db.Database.Migrate();
        logger.LogInformation("Database migration applied successfully.");
    }
}

app.Run();

