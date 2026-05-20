using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Casestudy.DAL;

var builder = WebApplication.CreateBuilder(args);

// CORS setup
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add services to the container
builder.Services.AddControllers();

// Database configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

if (!string.IsNullOrEmpty(databaseUrl))
{
    // Use PostgreSQL for production (Render)
    builder.Services.AddDbContext<AppDbContext>(c => c.UseNpgsql(databaseUrl));
}
else
{
    // Fallback to local SQL Server for development
    builder.Services.AddDbContext<AppDbContext>(c => c.UseSqlServer(connectionString));
}

// JWT Authentication
var secret = builder.Configuration.GetSection("AppSettings").GetValue<string>("Secret");
var key = Encoding.ASCII.GetBytes(secret!);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Content Security Policy headers
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] =
        "default-src 'self'; " +
        "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
        "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
        "font-src 'self' https://fonts.gstatic.com data:; " +
        "img-src 'self' data: https:; " +
        "connect-src 'self' https:; " +
        "frame-src 'self';";

    await next();
});

// Comment out HTTPS redirection for mobile dev mode compatibility
// app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);

// Serve static files from wwwroot
app.UseStaticFiles();

app.UseAuthentication(); // <-- Add this line
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("index.html");

// Auto-create database tables if they don't exist
try
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.EnsureCreated();
        Console.WriteLine("Database initialized successfully.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Database initialization error: {ex.Message}");
}

app.Run();
