using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Shared.Domain.DTOs;
using System.Diagnostics.Metrics;
using System.Diagnostics;

namespace AttendancePlatform.Monitoring.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MonitoringController : ControllerBase
{
    private readonly ILogger<MonitoringController> _logger;
    private readonly Meter _meter;
    private readonly Counter<int> _requestCounter;
    private readonly Histogram<double> _requestDuration;

    public MonitoringController(ILogger<MonitoringController> logger)
    {
        _logger = logger;
        _meter = new Meter("AttendancePlatform.Monitoring");
        _requestCounter = _meter.CreateCounter<int>("monitoring_requests_total", "Total number of monitoring requests");
        _requestDuration = _meter.CreateHistogram<double>("monitoring_request_duration_seconds", "Duration of monitoring requests");
    }

    [HttpGet("health")]
    [AllowAnonymous]
    public async Task<IActionResult> GetHealth()
    {
        using var activity = Activity.Current?.Source.StartActivity("MonitoringController.GetHealth");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _requestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "health"));

            var healthStatus = new
            {
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Service = "Monitoring",
                Version = "1.0.0"
            };

            _logger.LogInformation("Health check completed successfully");
            return Ok(healthStatus);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
            return StatusCode(500, new { Status = "Unhealthy", Error = ex.Message });
        }
        finally
        {
            _requestDuration.Record(stopwatch.Elapsed.TotalSeconds, new KeyValuePair<string, object?>("endpoint", "health"));
        }
    }

    [HttpGet("metrics")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMetrics()
    {
        using var activity = Activity.Current?.Source.StartActivity("MonitoringController.GetMetrics");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _requestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "metrics"));

            var metrics = new
            {
                SystemMetrics = new
                {
                    CpuUsage = GetCpuUsage(),
                    MemoryUsage = GetMemoryUsage(),
                    DiskUsage = GetDiskUsage()
                },
                ApplicationMetrics = new
                {
                    RequestCount = GetRequestCount(),
                    ErrorRate = GetErrorRate(),
                    ResponseTime = GetAverageResponseTime()
                },
                Timestamp = DateTime.UtcNow
            };

            return Ok(metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve metrics");
            return StatusCode(500, new { Error = ex.Message });
        }
        finally
        {
            _requestDuration.Record(stopwatch.Elapsed.TotalSeconds, new KeyValuePair<string, object?>("endpoint", "metrics"));
        }
    }

    [HttpGet("alerts")]
    public async Task<IActionResult> GetAlerts()
    {
        using var activity = Activity.Current?.Source.StartActivity("MonitoringController.GetAlerts");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _requestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "alerts"));

            var alerts = new[]
            {
                new
                {
                    Id = Guid.NewGuid(),
                    Severity = "Warning",
                    Message = "High CPU usage detected",
                    Timestamp = DateTime.UtcNow.AddMinutes(-5),
                    Status = "Active"
                },
                new
                {
                    Id = Guid.NewGuid(),
                    Severity = "Info",
                    Message = "System backup completed successfully",
                    Timestamp = DateTime.UtcNow.AddHours(-1),
                    Status = "Resolved"
                }
            };

            return Ok(alerts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve alerts");
            return StatusCode(500, new { Error = ex.Message });
        }
        finally
        {
            _requestDuration.Record(stopwatch.Elapsed.TotalSeconds, new KeyValuePair<string, object?>("endpoint", "alerts"));
        }
    }

    [HttpGet("traces")]
    public async Task<IActionResult> GetTraces()
    {
        using var activity = Activity.Current?.Source.StartActivity("MonitoringController.GetTraces");
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _requestCounter.Add(1, new KeyValuePair<string, object?>("endpoint", "traces"));

            var traces = new[]
            {
                new
                {
                    TraceId = Activity.Current?.TraceId.ToString(),
                    SpanId = Activity.Current?.SpanId.ToString(),
                    Operation = "MonitoringController.GetTraces",
                    Duration = "125ms",
                    Status = "Success",
                    Timestamp = DateTime.UtcNow
                }
            };

            return Ok(traces);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve traces");
            return StatusCode(500, new { Error = ex.Message });
        }
        finally
        {
            _requestDuration.Record(stopwatch.Elapsed.TotalSeconds, new KeyValuePair<string, object?>("endpoint", "traces"));
        }
    }

    private double GetCpuUsage()
    {
        return Random.Shared.NextDouble() * 100;
    }

    private double GetMemoryUsage()
    {
        var process = Process.GetCurrentProcess();
        return process.WorkingSet64 / (1024.0 * 1024.0);
    }

    private double GetDiskUsage()
    {
        return Random.Shared.NextDouble() * 100;
    }

    private int GetRequestCount()
    {
        return Random.Shared.Next(1000, 10000);
    }

    private double GetErrorRate()
    {
        return Random.Shared.NextDouble() * 5;
    }

    private double GetAverageResponseTime()
    {
        return Random.Shared.NextDouble() * 1000;
    }
}
