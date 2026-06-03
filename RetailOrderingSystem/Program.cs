/*
 * File: Program.cs
 * Purpose: Application entry point. Configures all services, middleware, and starts the app.
 *
 * REQUEST FLOW:
 * CLIENT
 *   ↓
 * RequestLoggingMiddleware  (logs every request)
 *   ↓
 * GlobalExceptionMiddleware (catches all errors)
 *   ↓
 * Rate Limiting             (prevents abuse)
 *   ↓
 * Authentication            (validates JWT token)
 *   ↓
 * Authorization             (checks role permissions)
 *   ↓
 * Controller                (handles HTTP request/response)
 *   ↓
 * Service                   (business logic)
 *   ↓
 * Repository / DbContext    (data access)
 *   ↓
 * SQL Server                (database)
 *   ↓
 * RESPONSE
 *
 * Interview Tip: Program.cs is the composition root — where all dependencies are wired together.
 *                Dependency Injection (DI) means classes receive their dependencies from outside
 *                rather than creating them internally. This makes code testable and loosely coupled.
 */

using AspNetCoreRateLimit;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using RetailOrderingSystem.Data;
using RetailOrderingSystem.Helpers;
using RetailOrderingSystem.Interfaces;
using RetailOrderingSystem.Middleware;
using RetailOrderingSystem.SeedData;
using RetailOrderingSystem.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. DATABASE — Entity Framework Core with SQL Server
// ============================================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================================================
// 2. AUTHENTICATION — JWT Bearer Token
// ============================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero  // No tolerance for expired tokens
    };
});

builder.Services.AddAuthorization();

// ============================================================
// 3. DEPENDENCY INJECTION — Register all services
// ============================================================
// Helpers
builder.Services.AddScoped<JwtHelper>();

// Services (business logic layer)
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<ILoyaltyService, LoyaltyService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IDeliveryService, DeliveryService>();

// ============================================================
// 4. FLUENT VALIDATION — Auto-validate request DTOs
// ============================================================
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// ============================================================
// 5. RATE LIMITING — Prevent API abuse
// ============================================================
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// ============================================================
// 6. SWAGGER — API documentation with JWT support
// ============================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Retail Ordering System API",
        Version = "v1",
        Description = "A complete retail ordering system with JWT auth, roles, cart, orders, loyalty points, and more.",
        Contact = new OpenApiContact { Name = "Retail System", Email = "admin@retail.com" }
    });

    // Add JWT authentication to Swagger UI
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer {your JWT token}\n\nExample: Bearer eyJhbGciOiJIUzI1NiIs..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ============================================================
// 7. CONTROLLERS + JSON settings
// ============================================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Return camelCase JSON (e.g., "firstName" not "FirstName")
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ============================================================
// 8. CORS — Allow all origins (configure for production)
// ============================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

// ============================================================
// BUILD THE APPLICATION
// ============================================================
var app = builder.Build();

// ============================================================
// 9. MIDDLEWARE PIPELINE (order matters!)
// ============================================================

// Global exception handler — must be FIRST to catch all errors
app.UseMiddleware<GlobalExceptionMiddleware>();

// Request logger — logs every request
app.UseMiddleware<RequestLoggingMiddleware>();

// Swagger UI — only in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Retail Ordering System v1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

// Rate limiting
app.UseIpRateLimiting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ============================================================
// 10. DATABASE MIGRATION + SEEDING
// ============================================================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Apply pending migrations automatically (creates DB if not exists)
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migration applied successfully.");

        // Seed initial data
        await DataSeeder.SeedAsync(context);
        logger.LogInformation("Database seeding completed.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error during database migration/seeding: {message}", ex.Message);
    }
}

app.Run();
