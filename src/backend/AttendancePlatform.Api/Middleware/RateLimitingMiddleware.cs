using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace AttendancePlatform.Api.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly ConcurrentDictionary<string, List<DateTime>> _requests = new();
        private readonly int _maxRequests = 100;
        private readonly TimeSpan _timeWindow = TimeSpan.FromMinutes(1);

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientId(context);
            var now = DateTime.UtcNow;

            var requests = _requests.GetOrAdd(clientId, _ => new List<DateTime>());

            lock (requests)
            {
                requests.RemoveAll(r => now - r > _timeWindow);
                
                if (requests.Count >= _maxRequests)
                {
                    _logger.LogWarning("Rate limit exceeded for client {ClientId}", clientId);
                    context.Response.StatusCode = 429;
                    context.Response.Headers.Add("Retry-After", "60");
                    await context.Response.WriteAsync("Rate limit exceeded");
                    return;
                }

                requests.Add(now);
            }

            await _next(context);
        }

        private string GetClientId(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }
    }
}
