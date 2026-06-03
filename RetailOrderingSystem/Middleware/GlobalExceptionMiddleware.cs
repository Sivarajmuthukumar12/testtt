/*
 * Folder: Middleware
 * File: GlobalExceptionMiddleware.cs
 * Purpose: Catches ALL unhandled exceptions in the pipeline and returns a
 *          consistent JSON error response. Without this, ASP.NET returns HTML errors.
 * Interview Tip: Middleware sits in the request pipeline. Every request passes through it.
 *                This is the "catch-all" safety net for the entire application.
 */

using System.Net;
using System.Text.Json;

namespace RetailOrderingSystem.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;  // The next middleware in the pipeline
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Pass request to the next middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Catch any unhandled exception and return a clean JSON response
                _logger.LogError(ex, "Unhandled exception: {message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            // Map exception types to HTTP status codes
            var (statusCode, errorCode) = ex switch
            {
                KeyNotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized, "UNAUTHORIZED"),
                InvalidOperationException => (HttpStatusCode.BadRequest, "BAD_REQUEST"),
                _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = new
            {
                Message = ex.Message,
                ErrorCode = errorCode,
                Timestamp = DateTime.UtcNow
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}
