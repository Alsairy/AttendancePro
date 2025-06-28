using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Services;
using System.Diagnostics;
using System.Text;
using System.IO.Compression;

namespace AttendancePlatform.Shared.Infrastructure.Middleware
{
    public class ApiPerformanceMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiPerformanceMiddleware> _logger;
        private readonly IApiPerformanceService _performanceService;

        public ApiPerformanceMiddleware(
            RequestDelegate next,
            ILogger<ApiPerformanceMiddleware> logger,
            IApiPerformanceService performanceService)
        {
            _next = next;
            _logger = logger;
            _performanceService = performanceService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var originalBodyStream = context.Response.Body;

            try
            {
                await OptimizeRequest(context);

                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                await OptimizeResponse(context, responseBody, originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in API performance middleware");
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                context.Response.Body = originalBodyStream;
                
                LogPerformanceMetrics(context, stopwatch.ElapsedMilliseconds);
            }
        }

        private async Task OptimizeRequest(HttpContext context)
        {
            context.Request.Headers.Add("X-Request-Start", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString());

            if (context.Request.Headers.ContainsKey("Accept-Encoding"))
            {
                var acceptEncoding = context.Request.Headers["Accept-Encoding"].ToString();
                if (acceptEncoding.Contains("gzip"))
                {
                    context.Response.Headers.Add("Vary", "Accept-Encoding");
                }
            }

            await _performanceService.OptimizeApiResponseAsync(context);
        }

        private async Task OptimizeResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
        {
            var response = context.Response;
            responseBody.Seek(0, SeekOrigin.Begin);

            if (ShouldCompress(context) && responseBody.Length > 1024)
            {
                await CompressAndWriteResponse(context, responseBody, originalBodyStream);
            }
            else
            {
                await responseBody.CopyToAsync(originalBodyStream);
            }

            AddPerformanceHeaders(context);
        }

        private bool ShouldCompress(HttpContext context)
        {
            var response = context.Response;
            var request = context.Request;

            if (response.StatusCode != 200) return false;
            if (!request.Headers.ContainsKey("Accept-Encoding")) return false;
            if (!request.Headers["Accept-Encoding"].ToString().Contains("gzip")) return false;

            var contentType = response.ContentType;
            if (string.IsNullOrEmpty(contentType)) return false;

            return contentType.Contains("application/json") ||
                   contentType.Contains("text/") ||
                   contentType.Contains("application/xml") ||
                   contentType.Contains("application/javascript");
        }

        private async Task CompressAndWriteResponse(HttpContext context, MemoryStream responseBody, Stream originalBodyStream)
        {
            context.Response.Headers.Add("Content-Encoding", "gzip");
            
            using var gzipStream = new GZipStream(originalBodyStream, CompressionLevel.Optimal, leaveOpen: true);
            await responseBody.CopyToAsync(gzipStream);
            await gzipStream.FlushAsync();

            var originalSize = responseBody.Length;
            var compressedSize = originalBodyStream.Length;
            var compressionRatio = (double)compressedSize / originalSize;

            _logger.LogDebug($"Response compressed: {originalSize} -> {compressedSize} bytes (ratio: {compressionRatio:P2})");
        }

        private void AddPerformanceHeaders(HttpContext context)
        {
            var response = context.Response;
            
            if (context.Request.Headers.TryGetValue("X-Request-Start", out var requestStartValue))
            {
                if (long.TryParse(requestStartValue, out var requestStart))
                {
                    var responseTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - requestStart;
                    response.Headers.Add("X-Response-Time", $"{responseTime}ms");
                }
            }

            response.Headers.Add("X-Powered-By", "Hudur AttendancePro");
            response.Headers.Add("X-Performance-Optimized", "true");

            if (ShouldAddCacheHeaders(context))
            {
                AddCacheHeaders(context);
            }

            AddSecurityHeaders(context);
        }

        private bool ShouldAddCacheHeaders(HttpContext context)
        {
            return context.Request.Method == "GET" && 
                   context.Response.StatusCode == 200 &&
                   !context.Request.Path.Value.Contains("/health") &&
                   !context.Request.Path.Value.Contains("/metrics");
        }

        private void AddCacheHeaders(HttpContext context)
        {
            var path = context.Request.Path.Value.ToLower();
            
            if (path.Contains("/api/users") || path.Contains("/api/settings"))
            {
                context.Response.Headers.Add("Cache-Control", "public, max-age=300");
                context.Response.Headers.Add("ETag", GenerateETag(context));
            }
            else if (path.Contains("/api/attendance") || path.Contains("/api/reports"))
            {
                context.Response.Headers.Add("Cache-Control", "public, max-age=60");
            }
            else if (path.Contains("/api/analytics") || path.Contains("/api/dashboard"))
            {
                context.Response.Headers.Add("Cache-Control", "public, max-age=30");
            }
            else
            {
                context.Response.Headers.Add("Cache-Control", "public, max-age=15");
            }
        }

        private void AddSecurityHeaders(HttpContext context)
        {
            var response = context.Response;
            
            if (!response.Headers.ContainsKey("X-Content-Type-Options"))
                response.Headers.Add("X-Content-Type-Options", "nosniff");
            
            if (!response.Headers.ContainsKey("X-Frame-Options"))
                response.Headers.Add("X-Frame-Options", "DENY");
            
            if (!response.Headers.ContainsKey("X-XSS-Protection"))
                response.Headers.Add("X-XSS-Protection", "1; mode=block");
            
            if (!response.Headers.ContainsKey("Referrer-Policy"))
                response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        }

        private string GenerateETag(HttpContext context)
        {
            var path = context.Request.Path.Value;
            var queryString = context.Request.QueryString.Value;
            var content = $"{path}{queryString}{DateTime.UtcNow:yyyyMMddHH}";
            
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hash)[..16];
        }

        private void LogPerformanceMetrics(HttpContext context, long responseTimeMs)
        {
            var request = context.Request;
            var response = context.Response;
            
            var logLevel = responseTimeMs > 1000 ? LogLevel.Warning : LogLevel.Debug;
            
            _logger.Log(logLevel, 
                "API Performance: {Method} {Path} -> {StatusCode} in {ResponseTime}ms",
                request.Method,
                request.Path,
                response.StatusCode,
                responseTimeMs);

            if (responseTimeMs > 5000)
            {
                _logger.LogWarning(
                    "Slow API detected: {Method} {Path} took {ResponseTime}ms - Consider optimization",
                    request.Method,
                    request.Path,
                    responseTimeMs);
            }
        }
    }
}
