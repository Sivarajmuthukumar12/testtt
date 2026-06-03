/*
 * Folder: Middleware
 * File: RequestLoggingMiddleware.cs
 * Purpose: Logs every HTTP request — method, path, status code, and elapsed time.
 * Interview Tip: This is a cross-cutting concern. Instead of logging in every controller,
 *                we log once in middleware for all requests.
 */

using System.Diagnostics;

namespace RetailOrderingSystem.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            await _next(context);  // Process the request

            stopwatch.Stop();

            _logger.LogInformation(
                "{method} {path} → {statusCode} ({elapsed}ms)",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
