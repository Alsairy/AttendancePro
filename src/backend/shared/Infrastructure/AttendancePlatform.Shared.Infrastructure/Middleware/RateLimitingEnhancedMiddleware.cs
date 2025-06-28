using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace AttendancePlatform.Shared.Infrastructure.Middleware
{
    public class RateLimitingEnhancedMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingEnhancedMiddleware> _logger;
        private readonly IMemoryCache _cache;
        private readonly RateLimitingOptions _options;

        public RateLimitingEnhancedMiddleware(
            RequestDelegate next,
            ILogger<RateLimitingEnhancedMiddleware> logger,
            IMemoryCache cache,
            RateLimitingOptions options)
        {
            _next = next;
            _logger = logger;
            _cache = cache;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);
            var endpoint = GetEndpointIdentifier(context);
            var rateLimitKey = $"rate_limit_{clientId}_{endpoint}";

            var rateLimitInfo = GetRateLimitInfo(rateLimitKey, endpoint);

            if (IsRateLimited(rateLimitInfo))
            {
                await HandleRateLimitExceeded(context, rateLimitInfo);
                return;
            }

            UpdateRateLimitInfo(rateLimitKey, rateLimitInfo);
            AddRateLimitHeaders(context, rateLimitInfo);

            await _next(context);
        }

        private string GetClientIdentifier(HttpContext context)
        {
            var clientId = context.Request.Headers["X-Client-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(clientId))
            {
                return clientId;
            }

            var userClaim = context.User?.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userClaim))
            {
                return $"user_{userClaim}";
            }

            var ipAddress = GetClientIpAddress(context);
            return $"ip_{ipAddress}";
        }

        private string GetClientIpAddress(HttpContext context)
        {
            var xForwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xForwardedFor))
            {
                return xForwardedFor.Split(',')[0].Trim();
            }

            var xRealIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(xRealIp))
            {
                return xRealIp;
            }

            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private string GetEndpointIdentifier(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method.ToUpper();

            if (path.StartsWith("/api/auth"))
                return "auth";
            if (path.StartsWith("/api/attendance"))
                return "attendance";
            if (path.StartsWith("/api/users"))
                return "users";
            if (path.StartsWith("/api/reports"))
                return "reports";
            if (path.StartsWith("/api/analytics"))
                return "analytics";
            if (path.StartsWith("/graphql"))
                return "graphql";

            return "general";
        }

        private RateLimitInfo GetRateLimitInfo(string key, string endpoint)
        {
            if (_cache.TryGetValue(key, out RateLimitInfo existingInfo))
            {
                return existingInfo;
            }

            var limits = GetEndpointLimits(endpoint);
            return new RateLimitInfo
            {
                RequestCount = 0,
                WindowStart = DateTime.UtcNow,
                WindowDurationMinutes = limits.WindowMinutes,
                RequestLimit = limits.RequestLimit,
                Endpoint = endpoint
            };
        }

        private EndpointLimits GetEndpointLimits(string endpoint)
        {
            return endpoint switch
            {
                "auth" => new EndpointLimits { RequestLimit = 10, WindowMinutes = 1 },
                "attendance" => new EndpointLimits { RequestLimit = 100, WindowMinutes = 1 },
                "users" => new EndpointLimits { RequestLimit = 50, WindowMinutes = 1 },
                "reports" => new EndpointLimits { RequestLimit = 20, WindowMinutes = 1 },
                "analytics" => new EndpointLimits { RequestLimit = 30, WindowMinutes = 1 },
                "graphql" => new EndpointLimits { RequestLimit = 200, WindowMinutes = 1 },
                _ => new EndpointLimits { RequestLimit = 60, WindowMinutes = 1 }
            };
        }

        private bool IsRateLimited(RateLimitInfo rateLimitInfo)
        {
            var windowExpired = DateTime.UtcNow > rateLimitInfo.WindowStart.AddMinutes(rateLimitInfo.WindowDurationMinutes);
            
            if (windowExpired)
            {
                rateLimitInfo.RequestCount = 0;
                rateLimitInfo.WindowStart = DateTime.UtcNow;
                return false;
            }

            return rateLimitInfo.RequestCount >= rateLimitInfo.RequestLimit;
        }

        private void UpdateRateLimitInfo(string key, RateLimitInfo rateLimitInfo)
        {
            rateLimitInfo.RequestCount++;
            rateLimitInfo.LastRequestTime = DateTime.UtcNow;

            var cacheExpiration = rateLimitInfo.WindowStart.AddMinutes(rateLimitInfo.WindowDurationMinutes + 1);
            _cache.Set(key, rateLimitInfo, cacheExpiration);
        }

        private void AddRateLimitHeaders(HttpContext context, RateLimitInfo rateLimitInfo)
        {
            var response = context.Response;
            
            response.Headers.Add("X-RateLimit-Limit", rateLimitInfo.RequestLimit.ToString());
            response.Headers.Add("X-RateLimit-Remaining", Math.Max(0, rateLimitInfo.RequestLimit - rateLimitInfo.RequestCount).ToString());
            
            var resetTime = rateLimitInfo.WindowStart.AddMinutes(rateLimitInfo.WindowDurationMinutes);
            var resetTimeUnix = ((DateTimeOffset)resetTime).ToUnixTimeSeconds();
            response.Headers.Add("X-RateLimit-Reset", resetTimeUnix.ToString());
            
            response.Headers.Add("X-RateLimit-Window", $"{rateLimitInfo.WindowDurationMinutes}m");
        }

        private async Task HandleRateLimitExceeded(HttpContext context, RateLimitInfo rateLimitInfo)
        {
            var clientId = GetClientIdentifier(context);
            _logger.LogWarning(
                "Rate limit exceeded for client {ClientId} on endpoint {Endpoint}. " +
                "Request count: {RequestCount}, Limit: {RequestLimit}",
                clientId, rateLimitInfo.Endpoint, rateLimitInfo.RequestCount, rateLimitInfo.RequestLimit);

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
            context.Response.ContentType = "application/json";

            var resetTime = rateLimitInfo.WindowStart.AddMinutes(rateLimitInfo.WindowDurationMinutes);
            var retryAfterSeconds = (int)(resetTime - DateTime.UtcNow).TotalSeconds;
            
            context.Response.Headers.Add("Retry-After", retryAfterSeconds.ToString());
            AddRateLimitHeaders(context, rateLimitInfo);

            var errorResponse = new
            {
                error = "Rate limit exceeded",
                message = $"Too many requests for endpoint '{rateLimitInfo.Endpoint}'. Please try again later.",
                retryAfter = retryAfterSeconds,
                limit = rateLimitInfo.RequestLimit,
                window = $"{rateLimitInfo.WindowDurationMinutes} minutes"
            };

            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class RateLimitingOptions
    {
        public bool Enabled { get; set; } = true;
        public Dictionary<string, EndpointLimits> EndpointLimits { get; set; } = new();
        public int DefaultRequestLimit { get; set; } = 60;
        public int DefaultWindowMinutes { get; set; } = 1;
        public bool EnableDistributedCache { get; set; } = false;
        public string RedisConnectionString { get; set; } = "";
    }

    public class EndpointLimits
    {
        public int RequestLimit { get; set; }
        public int WindowMinutes { get; set; }
    }

    public class RateLimitInfo
    {
        public int RequestCount { get; set; }
        public DateTime WindowStart { get; set; }
        public DateTime LastRequestTime { get; set; }
        public int WindowDurationMinutes { get; set; }
        public int RequestLimit { get; set; }
        public string Endpoint { get; set; } = "";
    }
}
