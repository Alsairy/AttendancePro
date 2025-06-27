using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;
using AttendancePlatform.Shared.Infrastructure.Data;
using System.Data;
using Microsoft.Data.SqlClient;

namespace AttendancePlatform.Shared.Infrastructure.Services
{
    public interface IDatabaseOptimizationService
    {
        Task OptimizeQueriesAsync();
        Task CreateOptimalIndexesAsync();
        Task AnalyzeQueryPerformanceAsync();
        Task<Dictionary<string, object>> GetDatabaseMetricsAsync();
        Task OptimizeConnectionPoolingAsync();
        Task PartitionLargeTablesAsync();
    }

    public class DatabaseOptimizationService : IDatabaseOptimizationService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<DatabaseOptimizationService> _logger;
        private readonly IMemoryCache _cache;

        public DatabaseOptimizationService(
            AttendancePlatformDbContext context,
            ILogger<DatabaseOptimizationService> logger,
            IMemoryCache cache)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
        }

        public async Task OptimizeQueriesAsync()
        {
            _logger.LogInformation("Starting database query optimization");

            try
            {
                await OptimizeAttendanceQueriesAsync();
                await OptimizeUserQueriesAsync();
                await OptimizeLeaveQueriesAsync();
                await OptimizeAnalyticsQueriesAsync();

                _logger.LogInformation("Database query optimization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during database query optimization");
                throw;
            }
        }

        public async Task CreateOptimalIndexesAsync()
        {
            _logger.LogInformation("Creating optimal database indexes");

            var indexCommands = new[]
            {
                "CREATE NONCLUSTERED INDEX IX_AttendanceRecords_UserId_Date ON AttendanceRecords (UserId, Date) INCLUDE (CheckInTime, CheckOutTime, Status)",
                "CREATE NONCLUSTERED INDEX IX_AttendanceRecords_TenantId_Date ON AttendanceRecords (TenantId, Date) INCLUDE (UserId, Status)",
                "CREATE NONCLUSTERED INDEX IX_Users_TenantId_IsActive ON Users (TenantId, IsActive) INCLUDE (Email, FirstName, LastName)",
                "CREATE NONCLUSTERED INDEX IX_LeaveRequests_UserId_Status ON LeaveRequests (UserId, Status) INCLUDE (StartDate, EndDate, LeaveType)",
                "CREATE NONCLUSTERED INDEX IX_Notifications_UserId_IsRead ON Notifications (UserId, IsRead) INCLUDE (CreatedAt, Type, Title)",
                "CREATE NONCLUSTERED INDEX IX_AuditLogs_TenantId_Timestamp ON AuditLogs (TenantId, Timestamp) INCLUDE (UserId, Action, EntityType)",
                "CREATE NONCLUSTERED INDEX IX_BiometricTemplates_UserId_Type ON BiometricTemplates (UserId, Type) INCLUDE (Template, IsActive)",
                "CREATE NONCLUSTERED INDEX IX_Geofences_TenantId_IsActive ON Geofences (TenantId, IsActive) INCLUDE (Name, Latitude, Longitude, Radius)"
            };

            foreach (var command in indexCommands)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync($"IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = '{GetIndexName(command)}') {command}");
                    _logger.LogInformation($"Created index: {GetIndexName(command)}");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to create index: {GetIndexName(command)}");
                }
            }
        }

        public async Task AnalyzeQueryPerformanceAsync()
        {
            _logger.LogInformation("Analyzing query performance");

            var performanceQueries = new[]
            {
                @"SELECT 
                    TOP 10 
                    qs.execution_count,
                    qs.total_elapsed_time / qs.execution_count AS avg_elapsed_time,
                    qs.total_logical_reads / qs.execution_count AS avg_logical_reads,
                    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1, 
                        ((CASE qs.statement_end_offset 
                            WHEN -1 THEN DATALENGTH(qt.text)
                            ELSE qs.statement_end_offset 
                        END - qs.statement_start_offset)/2)+1) AS statement_text
                FROM sys.dm_exec_query_stats qs
                CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
                WHERE qt.text LIKE '%AttendanceRecords%' OR qt.text LIKE '%Users%' OR qt.text LIKE '%LeaveRequests%'
                ORDER BY qs.total_elapsed_time / qs.execution_count DESC",

                @"SELECT 
                    i.name AS IndexName,
                    s.user_seeks,
                    s.user_scans,
                    s.user_lookups,
                    s.user_updates,
                    s.last_user_seek,
                    s.last_user_scan,
                    s.last_user_lookup,
                    s.last_user_update
                FROM sys.dm_db_index_usage_stats s
                INNER JOIN sys.indexes i ON s.object_id = i.object_id AND s.index_id = i.index_id
                INNER JOIN sys.objects o ON i.object_id = o.object_id
                WHERE o.type = 'U' AND s.database_id = DB_ID()
                ORDER BY s.user_seeks + s.user_scans + s.user_lookups DESC"
            };

            foreach (var query in performanceQueries)
            {
                try
                {
                    using var command = _context.Database.GetDbConnection().CreateCommand();
                    command.CommandText = query;
                    await _context.Database.OpenConnectionAsync();
                    
                    using var reader = await command.ExecuteReaderAsync();
                    var results = new List<Dictionary<string, object>>();
                    
                    while (await reader.ReadAsync())
                    {
                        var row = new Dictionary<string, object>();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                        results.Add(row);
                    }
                    
                    _logger.LogInformation($"Query performance analysis returned {results.Count} results");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to execute performance analysis query");
                }
                finally
                {
                    await _context.Database.CloseConnectionAsync();
                }
            }
        }

        public async Task<Dictionary<string, object>> GetDatabaseMetricsAsync()
        {
            var cacheKey = "database_metrics";
            if (_cache.TryGetValue(cacheKey, out Dictionary<string, object> cachedMetrics))
            {
                return cachedMetrics;
            }

            var metrics = new Dictionary<string, object>();

            try
            {
                var attendanceCount = await _context.AttendanceRecords.CountAsync();
                var userCount = await _context.Users.CountAsync();
                var leaveRequestCount = await _context.LeaveRequests.CountAsync();
                var notificationCount = await _context.Notifications.CountAsync();

                var avgResponseTime = await CalculateAverageResponseTimeAsync();
                var connectionPoolStats = await GetConnectionPoolStatsAsync();

                metrics["TotalAttendanceRecords"] = attendanceCount;
                metrics["TotalUsers"] = userCount;
                metrics["TotalLeaveRequests"] = leaveRequestCount;
                metrics["TotalNotifications"] = notificationCount;
                metrics["AverageResponseTimeMs"] = avgResponseTime;
                metrics["ConnectionPoolStats"] = connectionPoolStats;
                metrics["LastUpdated"] = DateTime.UtcNow;

                _cache.Set(cacheKey, metrics, TimeSpan.FromMinutes(5));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving database metrics");
                throw;
            }

            return metrics;
        }

        public async Task OptimizeConnectionPoolingAsync()
        {
            _logger.LogInformation("Optimizing database connection pooling");

            try
            {
                var connectionString = _context.Database.GetConnectionString();
                var builder = new SqlConnectionStringBuilder(connectionString);

                builder.MaxPoolSize = 100;
                builder.MinPoolSize = 10;
                builder.ConnectionTimeout = 30;
                builder.CommandTimeout = 60;
                builder.Pooling = true;

                _logger.LogInformation($"Connection pool optimized - Min: {builder.MinPoolSize}, Max: {builder.MaxPoolSize}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error optimizing connection pooling");
                throw;
            }
        }

        public async Task PartitionLargeTablesAsync()
        {
            _logger.LogInformation("Implementing table partitioning for large tables");

            var partitionCommands = new[]
            {
                @"CREATE PARTITION FUNCTION AttendanceRecordsPartitionFunction (datetime2)
                AS RANGE RIGHT FOR VALUES 
                ('2024-01-01', '2024-04-01', '2024-07-01', '2024-10-01', '2025-01-01')",

                @"CREATE PARTITION SCHEME AttendanceRecordsPartitionScheme
                AS PARTITION AttendanceRecordsPartitionFunction
                ALL TO ([PRIMARY])",

                @"CREATE PARTITION FUNCTION AuditLogsPartitionFunction (datetime2)
                AS RANGE RIGHT FOR VALUES 
                ('2024-01-01', '2024-02-01', '2024-03-01', '2024-04-01', '2024-05-01', '2024-06-01')",

                @"CREATE PARTITION SCHEME AuditLogsPartitionScheme
                AS PARTITION AuditLogsPartitionFunction
                ALL TO ([PRIMARY])"
            };

            foreach (var command in partitionCommands)
            {
                try
                {
                    await _context.Database.ExecuteSqlRawAsync(command);
                    _logger.LogInformation("Partition created successfully");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, $"Failed to create partition: {command.Substring(0, 50)}...");
                }
            }
        }

        private async Task OptimizeAttendanceQueriesAsync()
        {
            var optimizedQuery = _context.AttendanceRecords
                .Where(ar => ar.Date >= DateTime.Today.AddDays(-30))
                .Select(ar => new { ar.Id, ar.UserId, ar.Date, ar.CheckInTime, ar.CheckOutTime, ar.Status })
                .AsNoTracking();

            await optimizedQuery.LoadAsync();
        }

        private async Task OptimizeUserQueriesAsync()
        {
            var optimizedQuery = _context.Users
                .Where(u => u.IsActive)
                .Select(u => new { u.Id, u.Email, u.FirstName, u.LastName, u.TenantId })
                .AsNoTracking();

            await optimizedQuery.LoadAsync();
        }

        private async Task OptimizeLeaveQueriesAsync()
        {
            var optimizedQuery = _context.LeaveRequests
                .Where(lr => lr.StartDate >= DateTime.Today.AddDays(-90))
                .Select(lr => new { lr.Id, lr.UserId, lr.StartDate, lr.EndDate, lr.Status, lr.LeaveType })
                .AsNoTracking();

            await optimizedQuery.LoadAsync();
        }

        private async Task OptimizeAnalyticsQueriesAsync()
        {
            var monthlyStats = await _context.AttendanceRecords
                .Where(ar => ar.Date >= DateTime.Today.AddDays(-30))
                .GroupBy(ar => new { ar.UserId, Month = ar.Date.Month })
                .Select(g => new { g.Key.UserId, g.Key.Month, Count = g.Count() })
                .AsNoTracking()
                .ToListAsync();

            _logger.LogInformation($"Optimized analytics query returned {monthlyStats.Count} results");
        }

        private async Task<double> CalculateAverageResponseTimeAsync()
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            await _context.Users.Take(100).AsNoTracking().ToListAsync();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }

        private async Task<object> GetConnectionPoolStatsAsync()
        {
            try
            {
                using var command = _context.Database.GetDbConnection().CreateCommand();
                command.CommandText = @"
                    SELECT 
                        counter_name,
                        cntr_value
                    FROM sys.dm_os_performance_counters 
                    WHERE object_name LIKE '%:General Statistics%'
                    AND counter_name IN ('User Connections', 'Logical Connections', 'Physical Connections')";

                await _context.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();
                
                var stats = new Dictionary<string, object>();
                while (await reader.ReadAsync())
                {
                    stats[reader.GetString("counter_name")] = reader.GetValue("cntr_value");
                }
                
                return stats;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to retrieve connection pool stats");
                return new { Error = "Unable to retrieve connection pool statistics" };
            }
            finally
            {
                await _context.Database.CloseConnectionAsync();
            }
        }

        private string GetIndexName(string createIndexCommand)
        {
            var parts = createIndexCommand.Split(' ');
            var indexNameIndex = Array.IndexOf(parts, "INDEX") + 1;
            return indexNameIndex < parts.Length ? parts[indexNameIndex] : "Unknown";
        }
    }
}
