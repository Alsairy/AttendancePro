using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Text.Json;
using System.Collections.Concurrent;

namespace AttendancePlatform.Shared.Infrastructure.Services
{
    public interface IApiPerformanceService
    {
        Task<T> GetCachedResponseAsync<T>(string cacheKey, Func<Task<T>> dataFactory, TimeSpan? expiration = null);
        Task InvalidateCacheAsync(string pattern);
        Task<string> CompressResponseAsync(string content);
        Task<ApiPerformanceMetrics> GetPerformanceMetricsAsync();
        Task OptimizeApiResponseAsync(HttpContext context);
        Task<BatchResponse<T>> ProcessBatchRequestAsync<T>(IEnumerable<Func<Task<T>>> requests);
        Task PrewarmCacheAsync();
    }

    public class ApiPerformanceService : IApiPerformanceService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<ApiPerformanceService> _logger;
        private readonly ConcurrentDictionary<string, ApiCallMetrics> _apiMetrics;
        private readonly SemaphoreSlim _batchSemaphore;

        public ApiPerformanceService(
            IMemoryCache memoryCache,
            IDistributedCache distributedCache,
            ILogger<ApiPerformanceService> logger)
        {
            _memoryCache = memoryCache;
            _distributedCache = distributedCache;
            _logger = logger;
            _apiMetrics = new ConcurrentDictionary<string, ApiCallMetrics>();
            _batchSemaphore = new SemaphoreSlim(10, 10);
        }

        public async Task<T> GetCachedResponseAsync<T>(string cacheKey, Func<Task<T>> dataFactory, TimeSpan? expiration = null)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            try
            {
                if (_memoryCache.TryGetValue(cacheKey, out T cachedValue))
                {
                    _logger.LogDebug($"Cache hit for key: {cacheKey}");
                    RecordCacheMetrics(cacheKey, true, stopwatch.ElapsedMilliseconds);
                    return cachedValue;
                }

                var distributedCacheValue = await _distributedCache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(distributedCacheValue))
                {
                    var deserializedValue = JsonSerializer.Deserialize<T>(distributedCacheValue);
                    _memoryCache.Set(cacheKey, deserializedValue, TimeSpan.FromMinutes(5));
                    _logger.LogDebug($"Distributed cache hit for key: {cacheKey}");
                    RecordCacheMetrics(cacheKey, true, stopwatch.ElapsedMilliseconds);
                    return deserializedValue;
                }

                _logger.LogDebug($"Cache miss for key: {cacheKey}, fetching data");
                var data = await dataFactory();
                
                var cacheExpiration = expiration ?? TimeSpan.FromMinutes(15);
                _memoryCache.Set(cacheKey, data, cacheExpiration);
                
                var serializedData = JsonSerializer.Serialize(data);
                await _distributedCache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheExpiration
                });

                RecordCacheMetrics(cacheKey, false, stopwatch.ElapsedMilliseconds);
                return data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in cached response for key: {cacheKey}");
                return await dataFactory();
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public async Task InvalidateCacheAsync(string pattern)
        {
            try
            {
                _logger.LogInformation($"Invalidating cache with pattern: {pattern}");

                var memoryField = _memoryCache.GetType().GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (memoryField?.GetValue(_memoryCache) is IDictionary memoryDict)
                {
                    var keysToRemove = new List<object>();
                    foreach (DictionaryEntry entry in memoryDict)
                    {
                        if (entry.Key.ToString().Contains(pattern))
                        {
                            keysToRemove.Add(entry.Key);
                        }
                    }

                    foreach (var key in keysToRemove)
                    {
                        _memoryCache.Remove(key);
                    }
                }

                await Task.CompletedTask;
                _logger.LogInformation($"Cache invalidation completed for pattern: {pattern}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error invalidating cache with pattern: {pattern}");
            }
        }

        public async Task<string> CompressResponseAsync(string content)
        {
            try
            {
                if (string.IsNullOrEmpty(content) || content.Length < 1024)
                {
                    return content;
                }

                var bytes = System.Text.Encoding.UTF8.GetBytes(content);
                
                using var memoryStream = new MemoryStream();
                using (var gzipStream = new GZipStream(memoryStream, CompressionLevel.Optimal))
                {
                    await gzipStream.WriteAsync(bytes, 0, bytes.Length);
                }

                var compressedBytes = memoryStream.ToArray();
                var compressionRatio = (double)compressedBytes.Length / bytes.Length;
                
                _logger.LogDebug($"Content compressed from {bytes.Length} to {compressedBytes.Length} bytes (ratio: {compressionRatio:P2})");
                
                return Convert.ToBase64String(compressedBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error compressing response content");
                return content;
            }
        }

        public async Task<ApiPerformanceMetrics> GetPerformanceMetricsAsync()
        {
            try
            {
                var metrics = new ApiPerformanceMetrics
                {
                    TotalApiCalls = _apiMetrics.Values.Sum(m => m.CallCount),
                    AverageResponseTime = _apiMetrics.Values.Any() ? _apiMetrics.Values.Average(m => m.AverageResponseTime) : 0,
                    CacheHitRate = CalculateCacheHitRate(),
                    TopSlowEndpoints = GetTopSlowEndpoints(),
                    TopFrequentEndpoints = GetTopFrequentEndpoints(),
                    ErrorRate = CalculateErrorRate(),
                    ThroughputPerSecond = CalculateThroughput(),
                    MemoryCacheSize = GetMemoryCacheSize(),
                    LastUpdated = DateTime.UtcNow
                };

                return await Task.FromResult(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving API performance metrics");
                throw;
            }
        }

        public async Task OptimizeApiResponseAsync(HttpContext context)
        {
            try
            {
                var endpoint = context.Request.Path.Value;
                var method = context.Request.Method;
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();

                context.Response.Headers.Add("X-Response-Time", stopwatch.ElapsedMilliseconds.ToString());
                context.Response.Headers.Add("X-Cache-Status", "MISS");
                
                if (ShouldCompress(context))
                {
                    context.Response.Headers.Add("Content-Encoding", "gzip");
                }

                if (ShouldCache(context))
                {
                    var cacheControl = GetCacheControlHeader(endpoint);
                    context.Response.Headers.Add("Cache-Control", cacheControl);
                }

                context.Response.OnCompleted(() =>
                {
                    stopwatch.Stop();
                    RecordApiMetrics(endpoint, method, stopwatch.ElapsedMilliseconds, context.Response.StatusCode);
                    return Task.CompletedTask;
                });

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing API response");
            }
        }

        public async Task<BatchResponse<T>> ProcessBatchRequestAsync<T>(IEnumerable<Func<Task<T>>> requests)
        {
            var batchResponse = new BatchResponse<T>();
            var tasks = new List<Task<T>>();

            try
            {
                await _batchSemaphore.WaitAsync();

                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                foreach (var request in requests.Take(50))
                {
                    tasks.Add(ProcessSingleBatchRequest(request));
                }

                var results = await Task.WhenAll(tasks);
                
                stopwatch.Stop();

                batchResponse.Results = results.ToList();
                batchResponse.TotalCount = results.Length;
                batchResponse.ProcessingTimeMs = stopwatch.ElapsedMilliseconds;
                batchResponse.Success = true;

                _logger.LogInformation($"Batch request processed: {results.Length} items in {stopwatch.ElapsedMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch request");
                batchResponse.Success = false;
                batchResponse.ErrorMessage = ex.Message;
            }
            finally
            {
                _batchSemaphore.Release();
            }

            return batchResponse;
        }

        public async Task PrewarmCacheAsync()
        {
            _logger.LogInformation("Starting cache prewarming");

            try
            {
                var prewarmTasks = new List<Task>
                {
                    PrewarmUserDataAsync(),
                    PrewarmAttendanceDataAsync(),
                    PrewarmLeaveDataAsync(),
                    PrewarmSystemSettingsAsync(),
                    PrewarmAnalyticsDataAsync()
                };

                await Task.WhenAll(prewarmTasks);
                
                _logger.LogInformation("Cache prewarming completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during cache prewarming");
            }
        }

        private async Task<T> ProcessSingleBatchRequest<T>(Func<Task<T>> request)
        {
            try
            {
                return await request();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error processing single batch request");
                return default(T);
            }
        }

        private void RecordCacheMetrics(string cacheKey, bool isHit, long responseTimeMs)
        {
            var metricsKey = $"cache_{cacheKey}";
            _apiMetrics.AddOrUpdate(metricsKey, 
                new ApiCallMetrics 
                { 
                    CallCount = 1, 
                    AverageResponseTime = responseTimeMs,
                    CacheHits = isHit ? 1 : 0,
                    LastCalled = DateTime.UtcNow
                },
                (key, existing) => 
                {
                    existing.CallCount++;
                    existing.AverageResponseTime = (existing.AverageResponseTime + responseTimeMs) / 2;
                    if (isHit) existing.CacheHits++;
                    existing.LastCalled = DateTime.UtcNow;
                    return existing;
                });
        }

        private void RecordApiMetrics(string endpoint, string method, long responseTimeMs, int statusCode)
        {
            var metricsKey = $"{method}_{endpoint}";
            _apiMetrics.AddOrUpdate(metricsKey,
                new ApiCallMetrics
                {
                    CallCount = 1,
                    AverageResponseTime = responseTimeMs,
                    ErrorCount = statusCode >= 400 ? 1 : 0,
                    LastCalled = DateTime.UtcNow
                },
                (key, existing) =>
                {
                    existing.CallCount++;
                    existing.AverageResponseTime = (existing.AverageResponseTime + responseTimeMs) / 2;
                    if (statusCode >= 400) existing.ErrorCount++;
                    existing.LastCalled = DateTime.UtcNow;
                    return existing;
                });
        }

        private double CalculateCacheHitRate()
        {
            var cacheMetrics = _apiMetrics.Where(kvp => kvp.Key.StartsWith("cache_")).ToList();
            if (!cacheMetrics.Any()) return 0;

            var totalCalls = cacheMetrics.Sum(m => m.Value.CallCount);
            var totalHits = cacheMetrics.Sum(m => m.Value.CacheHits);
            
            return totalCalls > 0 ? (double)totalHits / totalCalls : 0;
        }

        private List<EndpointMetrics> GetTopSlowEndpoints()
        {
            return _apiMetrics
                .Where(kvp => !kvp.Key.StartsWith("cache_"))
                .OrderByDescending(kvp => kvp.Value.AverageResponseTime)
                .Take(10)
                .Select(kvp => new EndpointMetrics
                {
                    Endpoint = kvp.Key,
                    AverageResponseTime = kvp.Value.AverageResponseTime,
                    CallCount = kvp.Value.CallCount
                })
                .ToList();
        }

        private List<EndpointMetrics> GetTopFrequentEndpoints()
        {
            return _apiMetrics
                .Where(kvp => !kvp.Key.StartsWith("cache_"))
                .OrderByDescending(kvp => kvp.Value.CallCount)
                .Take(10)
                .Select(kvp => new EndpointMetrics
                {
                    Endpoint = kvp.Key,
                    AverageResponseTime = kvp.Value.AverageResponseTime,
                    CallCount = kvp.Value.CallCount
                })
                .ToList();
        }

        private double CalculateErrorRate()
        {
            var apiMetrics = _apiMetrics.Where(kvp => !kvp.Key.StartsWith("cache_")).ToList();
            if (!apiMetrics.Any()) return 0;

            var totalCalls = apiMetrics.Sum(m => m.Value.CallCount);
            var totalErrors = apiMetrics.Sum(m => m.Value.ErrorCount);
            
            return totalCalls > 0 ? (double)totalErrors / totalCalls : 0;
        }

        private double CalculateThroughput()
        {
            var recentMetrics = _apiMetrics
                .Where(kvp => !kvp.Key.StartsWith("cache_") && kvp.Value.LastCalled > DateTime.UtcNow.AddMinutes(-1))
                .ToList();

            return recentMetrics.Sum(m => m.Value.CallCount);
        }

        private long GetMemoryCacheSize()
        {
            try
            {
                var field = _memoryCache.GetType().GetField("_cache", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                if (field?.GetValue(_memoryCache) is IDictionary cache)
                {
                    return cache.Count * 1024;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to get memory cache size");
            }
            return 0;
        }

        private bool ShouldCompress(HttpContext context)
        {
            var contentType = context.Response.ContentType;
            return contentType != null && (
                contentType.Contains("application/json") ||
                contentType.Contains("text/") ||
                contentType.Contains("application/xml"));
        }

        private bool ShouldCache(HttpContext context)
        {
            return context.Request.Method == "GET" && 
                   context.Response.StatusCode == 200;
        }

        private string GetCacheControlHeader(string endpoint)
        {
            if (endpoint.Contains("/health") || endpoint.Contains("/metrics"))
                return "no-cache";
            
            if (endpoint.Contains("/users") || endpoint.Contains("/settings"))
                return "public, max-age=300";
            
            if (endpoint.Contains("/attendance") || endpoint.Contains("/reports"))
                return "public, max-age=60";
            
            return "public, max-age=30";
        }

        private async Task PrewarmUserDataAsync()
        {
            await GetCachedResponseAsync("active_users_all", async () =>
            {
                await Task.Delay(100);
                return new { Users = "Prewarmed user data" };
            }, TimeSpan.FromMinutes(30));
        }

        private async Task PrewarmAttendanceDataAsync()
        {
            await GetCachedResponseAsync("recent_attendance_summary", async () =>
            {
                await Task.Delay(100);
                return new { Attendance = "Prewarmed attendance data" };
            }, TimeSpan.FromMinutes(15));
        }

        private async Task PrewarmLeaveDataAsync()
        {
            await GetCachedResponseAsync("pending_leave_requests_summary", async () =>
            {
                await Task.Delay(100);
                return new { Leave = "Prewarmed leave data" };
            }, TimeSpan.FromMinutes(10));
        }

        private async Task PrewarmSystemSettingsAsync()
        {
            await GetCachedResponseAsync("system_settings_all", async () =>
            {
                await Task.Delay(100);
                return new { Settings = "Prewarmed system settings" };
            }, TimeSpan.FromHours(1));
        }

        private async Task PrewarmAnalyticsDataAsync()
        {
            await GetCachedResponseAsync("dashboard_analytics_summary", async () =>
            {
                await Task.Delay(100);
                return new { Analytics = "Prewarmed analytics data" };
            }, TimeSpan.FromMinutes(5));
        }
    }

    public class ApiPerformanceMetrics
    {
        public long TotalApiCalls { get; set; }
        public double AverageResponseTime { get; set; }
        public double CacheHitRate { get; set; }
        public List<EndpointMetrics> TopSlowEndpoints { get; set; } = new();
        public List<EndpointMetrics> TopFrequentEndpoints { get; set; } = new();
        public double ErrorRate { get; set; }
        public double ThroughputPerSecond { get; set; }
        public long MemoryCacheSize { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class EndpointMetrics
    {
        public string Endpoint { get; set; }
        public double AverageResponseTime { get; set; }
        public long CallCount { get; set; }
    }

    public class ApiCallMetrics
    {
        public long CallCount { get; set; }
        public double AverageResponseTime { get; set; }
        public long CacheHits { get; set; }
        public long ErrorCount { get; set; }
        public DateTime LastCalled { get; set; }
    }

    public class BatchResponse<T>
    {
        public List<T> Results { get; set; } = new();
        public int TotalCount { get; set; }
        public long ProcessingTimeMs { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
