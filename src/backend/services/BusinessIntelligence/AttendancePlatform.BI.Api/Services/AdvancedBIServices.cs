using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using System.Text.Json;
using OfficeOpenXml;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace AttendancePlatform.BI.Api.Services
{
    public class DataVisualizationService : IDataVisualizationService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<DataVisualizationService> _logger;

        public DataVisualizationService(AttendancePlatformDbContext context, ILogger<DataVisualizationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ChartDataDto> GenerateChartDataAsync(ChartConfigurationDto config)
        {
            try
            {
                var data = await GetChartData(config);
                
                return new ChartDataDto
                {
                    ChartType = config.ChartType,
                    Data = data,
                    Configuration = new Dictionary<string, object>
                    {
                        { "xAxis", config.XAxis },
                        { "yAxis", config.YAxis },
                        { "groupBy", config.GroupBy ?? "" },
                        { "aggregateFunction", config.AggregateFunction ?? "count" }
                    },
                    GeneratedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating chart data");
                throw;
            }
        }

        public async Task<List<ChartTypeDto>> GetAvailableChartTypesAsync()
        {
            return new List<ChartTypeDto>
            {
                new ChartTypeDto
                {
                    Id = "line",
                    Name = "Line Chart",
                    Description = "Show trends over time",
                    SupportedDataTypes = new List<string> { "datetime", "number" },
                    ConfigurationSchema = new Dictionary<string, object>
                    {
                        { "smooth", new { type = "boolean", default = false } },
                        { "showPoints", new { type = "boolean", default = true } }
                    }
                },
                new ChartTypeDto
                {
                    Id = "bar",
                    Name = "Bar Chart",
                    Description = "Compare values across categories",
                    SupportedDataTypes = new List<string> { "string", "number" },
                    ConfigurationSchema = new Dictionary<string, object>
                    {
                        { "orientation", new { type = "select", options = new[] { "vertical", "horizontal" } } },
                        { "stacked", new { type = "boolean", default = false } }
                    }
                },
                new ChartTypeDto
                {
                    Id = "pie",
                    Name = "Pie Chart",
                    Description = "Show proportions of a whole",
                    SupportedDataTypes = new List<string> { "string", "number" },
                    ConfigurationSchema = new Dictionary<string, object>
                    {
                        { "showLabels", new { type = "boolean", default = true } },
                        { "showPercentages", new { type = "boolean", default = true } }
                    }
                },
                new ChartTypeDto
                {
                    Id = "area",
                    Name = "Area Chart",
                    Description = "Show cumulative totals over time",
                    SupportedDataTypes = new List<string> { "datetime", "number" },
                    ConfigurationSchema = new Dictionary<string, object>
                    {
                        { "stacked", new { type = "boolean", default = false } },
                        { "fillOpacity", new { type = "number", default = 0.6 } }
                    }
                },
                new ChartTypeDto
                {
                    Id = "scatter",
                    Name = "Scatter Plot",
                    Description = "Show correlation between two variables",
                    SupportedDataTypes = new List<string> { "number", "number" },
                    ConfigurationSchema = new Dictionary<string, object>
                    {
                        { "showTrendline", new { type = "boolean", default = false } },
                        { "pointSize", new { type = "number", default = 5 } }
                    }
                },
                new ChartTypeDto
                {
                    Id = "heatmap",
                    Name = "Heatmap",
                    Description = "Show data density and patterns",
                    SupportedDataTypes = new List<string> { "string", "string", "number" },
                    ConfigurationSchema = new Dictionary<string, object>
                    {
                        { "colorScheme", new { type = "select", options = new[] { "blues", "reds", "greens" } } },
                        { "showValues", new { type = "boolean", default = true } }
                    }
                }
            };
        }

        public async Task<byte[]> ExportChartAsync(Guid chartId, string format)
        {
            try
            {
                // In a real implementation, this would retrieve the chart configuration
                // and generate the chart image in the specified format
                var chartData = new byte[0]; // Placeholder
                
                return format.ToLower() switch
                {
                    "png" => await GenerateChartPng(chartId),
                    "svg" => await GenerateChartSvg(chartId),
                    "pdf" => await GenerateChartPdf(chartId),
                    _ => throw new ArgumentException("Unsupported format")
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting chart {ChartId} to {Format}", chartId, format);
                throw;
            }
        }

        public async Task<DrillDownDataDto> GetDrillDownDataAsync(string chartType, Dictionary<string, object> filters)
        {
            try
            {
                var data = await GetDrillDownData(filters);
                
                return new DrillDownDataDto
                {
                    Level = DetermineDrillDownLevel(filters),
                    Data = data,
                    Filters = filters
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting drill-down data");
                throw;
            }
        }

        private async Task<List<Dictionary<string, object>>> GetChartData(ChartConfigurationDto config)
        {
            var results = new List<Dictionary<string, object>>();

            if (config.DataSource == "AttendanceRecords")
            {
                var query = _context.AttendanceRecords.AsQueryable();

                // Apply filters
                foreach (var filter in config.Filters)
                {
                    query = ApplyFilter(query, filter.Key, filter.Value);
                }

                var data = await query.Include(ar => ar.User).ToListAsync();

                // Group and aggregate data based on configuration
                if (!string.IsNullOrEmpty(config.GroupBy))
                {
                    results = GroupAndAggregateData(data, config);
                }
                else
                {
                    results = data.Select(ar => new Dictionary<string, object>
                    {
                        { config.XAxis, GetPropertyValue(ar, config.XAxis) },
                        { config.YAxis, GetPropertyValue(ar, config.YAxis) }
                    }).ToList();
                }
            }

            return results;
        }

        private IQueryable<AttendanceRecord> ApplyFilter(IQueryable<AttendanceRecord> query, string filterKey, object filterValue)
        {
            return filterKey switch
            {
                "dateRange" => ApplyDateRangeFilter(query, filterValue.ToString()),
                "department" => query.Where(ar => ar.User.Department == filterValue.ToString()),
                "userId" => query.Where(ar => ar.UserId == Guid.Parse(filterValue.ToString())),
                _ => query
            };
        }

        private IQueryable<AttendanceRecord> ApplyDateRangeFilter(IQueryable<AttendanceRecord> query, string dateRange)
        {
            var (start, end) = ParseDateRange(dateRange);
            return query.Where(ar => ar.CheckInTime >= start && ar.CheckInTime <= end);
        }

        private (DateTime Start, DateTime End) ParseDateRange(string dateRange)
        {
            var now = DateTime.UtcNow;
            return dateRange switch
            {
                "today" => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
                "week" => (now.Date.AddDays(-(int)now.DayOfWeek), now.Date.AddDays(7 - (int)now.DayOfWeek).AddTicks(-1)),
                "month" => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)),
                "quarter" => GetQuarterRange(now),
                "year" => (new DateTime(now.Year, 1, 1), new DateTime(now.Year + 1, 1, 1).AddTicks(-1)),
                _ => (now.Date.AddDays(-30), now.Date.AddDays(1).AddTicks(-1))
            };
        }

        private (DateTime Start, DateTime End) GetQuarterRange(DateTime date)
        {
            var quarter = (date.Month - 1) / 3 + 1;
            var startMonth = (quarter - 1) * 3 + 1;
            var start = new DateTime(date.Year, startMonth, 1);
            var end = start.AddMonths(3).AddTicks(-1);
            return (start, end);
        }

        private List<Dictionary<string, object>> GroupAndAggregateData(List<AttendanceRecord> data, ChartConfigurationDto config)
        {
            var grouped = data.GroupBy(ar => GetPropertyValue(ar, config.GroupBy));
            
            return grouped.Select(g => new Dictionary<string, object>
            {
                { config.XAxis, g.Key },
                { config.YAxis, ApplyAggregateFunction(g.ToList(), config.YAxis, config.AggregateFunction) }
            }).ToList();
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            return property?.GetValue(obj) ?? "";
        }

        private object ApplyAggregateFunction(List<AttendanceRecord> records, string field, string function)
        {
            return function?.ToLower() switch
            {
                "count" => records.Count,
                "sum" => records.Sum(r => Convert.ToDouble(GetPropertyValue(r, field) ?? 0)),
                "avg" => records.Average(r => Convert.ToDouble(GetPropertyValue(r, field) ?? 0)),
                "min" => records.Min(r => Convert.ToDouble(GetPropertyValue(r, field) ?? 0)),
                "max" => records.Max(r => Convert.ToDouble(GetPropertyValue(r, field) ?? 0)),
                _ => records.Count
            };
        }

        private async Task<List<Dictionary<string, object>>> GetDrillDownData(Dictionary<string, object> filters)
        {
            // Implementation for drill-down data retrieval
            var results = new List<Dictionary<string, object>>();
            
            // This would contain logic to get more detailed data based on the current filters
            // For example, if drilling down from department to individual users
            
            return results;
        }

        private string DetermineDrillDownLevel(Dictionary<string, object> filters)
        {
            // Logic to determine the current drill-down level
            if (filters.ContainsKey("userId")) return "individual";
            if (filters.ContainsKey("department")) return "department";
            return "organization";
        }

        private async Task<byte[]> GenerateChartPng(Guid chartId)
        {
            // Implementation for PNG chart generation
            return new byte[0];
        }

        private async Task<byte[]> GenerateChartSvg(Guid chartId)
        {
            // Implementation for SVG chart generation
            return new byte[0];
        }

        private async Task<byte[]> GenerateChartPdf(Guid chartId)
        {
            // Implementation for PDF chart generation
            return new byte[0];
        }
    }

    public class KPIService : IKPIService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<KPIService> _logger;

        public KPIService(AttendancePlatformDbContext context, ILogger<KPIService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<KPIDto>> GetKPIsAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                var kpis = new List<KPIDto>();

                // Attendance Rate KPI
                var attendanceRate = await CalculateAttendanceRate(tenantId, fromDate, toDate);
                kpis.Add(new KPIDto
                {
                    Id = "attendance-rate",
                    Name = "Attendance Rate",
                    Category = "Attendance",
                    Value = Math.Round(attendanceRate, 2),
                    Target = 95.0,
                    Unit = "%",
                    Trend = await GetKPITrend("attendance-rate", tenantId, 7),
                    ChangePercentage = await GetKPIChange("attendance-rate", tenantId, 7),
                    Status = GetKPIStatus(attendanceRate, 95.0),
                    CalculatedAt = DateTime.UtcNow
                });

                // Average Work Hours KPI
                var avgWorkHours = await CalculateAverageWorkHours(tenantId, fromDate, toDate);
                kpis.Add(new KPIDto
                {
                    Id = "avg-work-hours",
                    Name = "Average Work Hours",
                    Category = "Productivity",
                    Value = Math.Round(avgWorkHours, 2),
                    Target = 8.0,
                    Unit = "hours",
                    Trend = await GetKPITrend("avg-work-hours", tenantId, 7),
                    ChangePercentage = await GetKPIChange("avg-work-hours", tenantId, 7),
                    Status = GetKPIStatus(avgWorkHours, 8.0, 0.5),
                    CalculatedAt = DateTime.UtcNow
                });

                // Punctuality Rate KPI
                var punctualityRate = await CalculatePunctualityRate(tenantId, fromDate, toDate);
                kpis.Add(new KPIDto
                {
                    Id = "punctuality-rate",
                    Name = "Punctuality Rate",
                    Category = "Attendance",
                    Value = Math.Round(punctualityRate, 2),
                    Target = 90.0,
                    Unit = "%",
                    Trend = await GetKPITrend("punctuality-rate", tenantId, 7),
                    ChangePercentage = await GetKPIChange("punctuality-rate", tenantId, 7),
                    Status = GetKPIStatus(punctualityRate, 90.0),
                    CalculatedAt = DateTime.UtcNow
                });

                // Overtime Rate KPI
                var overtimeRate = await CalculateOvertimeRate(tenantId, fromDate, toDate);
                kpis.Add(new KPIDto
                {
                    Id = "overtime-rate",
                    Name = "Overtime Rate",
                    Category = "Productivity",
                    Value = Math.Round(overtimeRate, 2),
                    Target = 10.0,
                    Unit = "%",
                    Trend = await GetKPITrend("overtime-rate", tenantId, 7),
                    ChangePercentage = await GetKPIChange("overtime-rate", tenantId, 7),
                    Status = GetKPIStatus(overtimeRate, 10.0, isLowerBetter: true),
                    CalculatedAt = DateTime.UtcNow
                });

                // Leave Utilization KPI
                var leaveUtilization = await CalculateLeaveUtilization(tenantId, fromDate, toDate);
                kpis.Add(new KPIDto
                {
                    Id = "leave-utilization",
                    Name = "Leave Utilization",
                    Category = "Leave",
                    Value = Math.Round(leaveUtilization, 2),
                    Target = 75.0,
                    Unit = "%",
                    Trend = await GetKPITrend("leave-utilization", tenantId, 30),
                    ChangePercentage = await GetKPIChange("leave-utilization", tenantId, 30),
                    Status = GetKPIStatus(leaveUtilization, 75.0, 10.0),
                    CalculatedAt = DateTime.UtcNow
                });

                return kpis;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting KPIs for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<KPIDto> CreateCustomKPIAsync(CreateKPIRequestDto request)
        {
            try
            {
                // In a real implementation, this would parse and execute the custom formula
                var value = await ExecuteCustomFormula(request.Formula, request.TenantId);

                return new KPIDto
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = request.Name,
                    Category = request.Category,
                    Value = value,
                    Target = request.Target,
                    Unit = request.Unit,
                    Trend = "stable",
                    Status = GetKPIStatus(Convert.ToDouble(value), Convert.ToDouble(request.Target ?? 0)),
                    CalculatedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating custom KPI");
                throw;
            }
        }

        public async Task<List<KPITrendDto>> GetKPITrendsAsync(Guid tenantId, string kpiType, int days)
        {
            try
            {
                var trends = new List<KPITrendDto>();
                var endDate = DateTime.UtcNow.Date;
                var startDate = endDate.AddDays(-days);

                for (var date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    var value = await CalculateKPIForDate(tenantId, kpiType, date);
                    var target = GetKPITarget(kpiType);

                    trends.Add(new KPITrendDto
                    {
                        Date = date,
                        Value = value,
                        Target = target
                    });
                }

                return trends;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting KPI trends for {KpiType}", kpiType);
                throw;
            }
        }

        public async Task<KPIBenchmarkDto> GetKPIBenchmarksAsync(Guid tenantId)
        {
            try
            {
                // In a real implementation, this would fetch industry benchmarks from external sources
                return new KPIBenchmarkDto
                {
                    Industry = "Technology",
                    CompanySize = "Medium (100-500 employees)",
                    Benchmarks = new Dictionary<string, object>
                    {
                        { "attendance-rate", new { industry = 94.5, topQuartile = 97.2, median = 93.8 } },
                        { "punctuality-rate", new { industry = 88.3, topQuartile = 94.1, median = 87.5 } },
                        { "overtime-rate", new { industry = 12.4, topQuartile = 8.2, median = 13.1 } },
                        { "leave-utilization", new { industry = 72.8, topQuartile = 78.5, median = 71.2 } }
                    },
                    LastUpdated = DateTime.UtcNow.AddDays(-7)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting KPI benchmarks");
                throw;
            }
        }

        // Helper methods for KPI calculations
        private async Task<double> CalculateAttendanceRate(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            var totalEmployees = await _context.Users.Where(u => u.TenantId == tenantId && u.IsActive).CountAsync();
            var workingDays = CalculateWorkingDays(fromDate, toDate);
            var expectedAttendance = totalEmployees * workingDays;
            
            var actualAttendance = await _context.AttendanceRecords
                .Where(ar => ar.TenantId == tenantId && ar.CheckInTime >= fromDate && ar.CheckInTime <= toDate)
                .CountAsync();

            return expectedAttendance > 0 ? (double)actualAttendance / expectedAttendance * 100 : 0;
        }

        private async Task<double> CalculateAverageWorkHours(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            var records = await _context.AttendanceRecords
                .Where(ar => ar.TenantId == tenantId && 
                           ar.CheckInTime >= fromDate && 
                           ar.CheckInTime <= toDate && 
                           ar.CheckOutTime.HasValue)
                .ToListAsync();

            if (!records.Any()) return 0;

            return records.Average(r => (r.CheckOutTime.Value - r.CheckInTime).TotalHours);
        }

        private async Task<double> CalculatePunctualityRate(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            var records = await _context.AttendanceRecords
                .Where(ar => ar.TenantId == tenantId && ar.CheckInTime >= fromDate && ar.CheckInTime <= toDate)
                .ToListAsync();

            if (!records.Any()) return 0;

            var onTimeRecords = records.Count(r => r.CheckInTime.TimeOfDay <= TimeSpan.FromHours(9));
            return (double)onTimeRecords / records.Count * 100;
        }

        private async Task<double> CalculateOvertimeRate(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            var records = await _context.AttendanceRecords
                .Where(ar => ar.TenantId == tenantId && 
                           ar.CheckInTime >= fromDate && 
                           ar.CheckInTime <= toDate && 
                           ar.CheckOutTime.HasValue)
                .ToListAsync();

            if (!records.Any()) return 0;

            var overtimeRecords = records.Count(r => (r.CheckOutTime.Value - r.CheckInTime).TotalHours > 8);
            return (double)overtimeRecords / records.Count * 100;
        }

        private async Task<double> CalculateLeaveUtilization(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            var users = await _context.Users
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .Include(u => u.LeaveRequests.Where(lr => lr.StartDate >= fromDate && lr.EndDate <= toDate))
                .ToListAsync();

            if (!users.Any()) return 0;

            var totalLeaveUsed = users.Sum(u => u.LeaveRequests.Sum(lr => (lr.EndDate - lr.StartDate).Days + 1));
            var totalLeaveEntitlement = users.Count * 25; // Assuming 25 days annual leave

            return (double)totalLeaveUsed / totalLeaveEntitlement * 100;
        }

        private async Task<string> GetKPITrend(string kpiType, Guid tenantId, int days)
        {
            var currentValue = await CalculateKPIForDate(tenantId, kpiType, DateTime.UtcNow.Date);
            var previousValue = await CalculateKPIForDate(tenantId, kpiType, DateTime.UtcNow.Date.AddDays(-days));

            if (currentValue > previousValue) return "increasing";
            if (currentValue < previousValue) return "decreasing";
            return "stable";
        }

        private async Task<double?> GetKPIChange(string kpiType, Guid tenantId, int days)
        {
            var currentValue = await CalculateKPIForDate(tenantId, kpiType, DateTime.UtcNow.Date);
            var previousValue = await CalculateKPIForDate(tenantId, kpiType, DateTime.UtcNow.Date.AddDays(-days));

            if (previousValue == 0) return null;
            return ((currentValue - previousValue) / previousValue) * 100;
        }

        private async Task<double> CalculateKPIForDate(Guid tenantId, string kpiType, DateTime date)
        {
            var fromDate = date;
            var toDate = date.AddDays(1).AddTicks(-1);

            return kpiType switch
            {
                "attendance-rate" => await CalculateAttendanceRate(tenantId, fromDate, toDate),
                "avg-work-hours" => await CalculateAverageWorkHours(tenantId, fromDate, toDate),
                "punctuality-rate" => await CalculatePunctualityRate(tenantId, fromDate, toDate),
                "overtime-rate" => await CalculateOvertimeRate(tenantId, fromDate, toDate),
                "leave-utilization" => await CalculateLeaveUtilization(tenantId, fromDate, toDate),
                _ => 0
            };
        }

        private object GetKPITarget(string kpiType)
        {
            return kpiType switch
            {
                "attendance-rate" => 95.0,
                "avg-work-hours" => 8.0,
                "punctuality-rate" => 90.0,
                "overtime-rate" => 10.0,
                "leave-utilization" => 75.0,
                _ => 0
            };
        }

        private string GetKPIStatus(double value, double target, double tolerance = 5.0, bool isLowerBetter = false)
        {
            var difference = Math.Abs(value - target);
            
            if (difference <= tolerance)
                return "on-target";
            
            if (isLowerBetter)
                return value < target ? "above-target" : "below-target";
            else
                return value > target ? "above-target" : "below-target";
        }

        private int CalculateWorkingDays(DateTime fromDate, DateTime toDate)
        {
            int workingDays = 0;
            for (var date = fromDate.Date; date <= toDate.Date; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }
            return workingDays;
        }

        private async Task<object> ExecuteCustomFormula(string formula, Guid tenantId)
        {
            // Simplified formula execution - in a real implementation, this would be more sophisticated
            // For now, return a placeholder value
            return 85.5;
        }
    }

    public class AdvancedReportingService : IAdvancedReportingService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<AdvancedReportingService> _logger;

        public AdvancedReportingService(AttendancePlatformDbContext context, ILogger<AdvancedReportingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ScheduledReportDto> ScheduleReportAsync(ScheduleReportRequestDto request)
        {
            try
            {
                var scheduledReport = new ScheduledReport
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    ReportId = request.ReportId,
                    Name = request.Name,
                    CronExpression = request.CronExpression,
                    Recipients = JsonSerializer.Serialize(request.Recipients),
                    Format = request.Format,
                    Parameters = JsonSerializer.Serialize(request.Parameters),
                    IsActive = request.IsActive,
                    NextExecution = CalculateNextExecution(request.CronExpression),
                    CreatedAt = DateTime.UtcNow
                };

                _context.ScheduledReports.Add(scheduledReport);
                await _context.SaveChangesAsync();

                return MapToScheduledReportDto(scheduledReport);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling report");
                throw;
            }
        }

        public async Task<List<ScheduledReportDto>> GetScheduledReportsAsync(Guid tenantId)
        {
            try
            {
                var reports = await _context.ScheduledReports
                    .Where(sr => sr.TenantId == tenantId)
                    .OrderBy(sr => sr.NextExecution)
                    .ToListAsync();

                return reports.Select(MapToScheduledReportDto).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting scheduled reports");
                throw;
            }
        }

        public async Task<ReportExecutionResultDto> ExecuteScheduledReportAsync(Guid scheduledReportId)
        {
            try
            {
                var scheduledReport = await _context.ScheduledReports.FindAsync(scheduledReportId);
                if (scheduledReport == null)
                    throw new ArgumentException("Scheduled report not found");

                var startTime = DateTime.UtcNow;
                var success = false;
                string errorMessage = null;
                int rowCount = 0;

                try
                {
                    // Execute the report
                    var parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(scheduledReport.Parameters);
                    var reportData = await ExecuteReport(scheduledReport.ReportId, parameters);
                    rowCount = reportData.Count;

                    // Generate the report file
                    var reportBytes = await GenerateReportFile(reportData, scheduledReport.Format);

                    // Send to recipients
                    var recipients = JsonSerializer.Deserialize<List<string>>(scheduledReport.Recipients);
                    await SendReportToRecipients(reportBytes, scheduledReport.Name, scheduledReport.Format, recipients);

                    success = true;
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    _logger.LogError(ex, "Error executing scheduled report {ReportId}", scheduledReportId);
                }

                var executionTime = (long)(DateTime.UtcNow - startTime).TotalMilliseconds;

                // Update last execution time and calculate next execution
                scheduledReport.LastExecuted = DateTime.UtcNow;
                scheduledReport.NextExecution = CalculateNextExecution(scheduledReport.CronExpression);
                await _context.SaveChangesAsync();

                return new ReportExecutionResultDto
                {
                    ExecutionId = Guid.NewGuid(),
                    Success = success,
                    ErrorMessage = errorMessage,
                    RowCount = success ? rowCount : null,
                    ExecutionTimeMs = executionTime,
                    ExecutedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing scheduled report {ReportId}", scheduledReportId);
                throw;
            }
        }

        public async Task<byte[]> GenerateAdvancedReportAsync(AdvancedReportRequestDto request)
        {
            try
            {
                var data = await GetReportData(request);
                return await GenerateReportFile(data, request.Format);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating advanced report");
                throw;
            }
        }

        public async Task<List<ReportExecutionHistoryDto>> GetReportExecutionHistoryAsync(Guid reportId)
        {
            try
            {
                var history = await _context.ReportExecutionHistory
                    .Where(reh => reh.ReportId == reportId)
                    .OrderByDescending(reh => reh.ExecutedAt)
                    .Take(50)
                    .ToListAsync();

                return history.Select(h => new ReportExecutionHistoryDto
                {
                    Id = h.Id,
                    ExecutedAt = h.ExecutedAt,
                    Parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(h.Parameters),
                    RowCount = h.RowCount,
                    ExecutionTimeMs = h.ExecutionTimeMs,
                    Success = true, // Simplified for this example
                    ErrorMessage = null
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting report execution history");
                throw;
            }
        }

        private async Task<List<Dictionary<string, object>>> ExecuteReport(Guid reportId, Dictionary<string, object> parameters)
        {
            // Implementation would execute the report and return data
            return new List<Dictionary<string, object>>();
        }

        private async Task<List<Dictionary<string, object>>> GetReportData(AdvancedReportRequestDto request)
        {
            // Implementation would get data based on report type and parameters
            return new List<Dictionary<string, object>>();
        }

        private async Task<byte[]> GenerateReportFile(List<Dictionary<string, object>> data, string format)
        {
            return format.ToUpper() switch
            {
                "PDF" => await GeneratePdfReport(data),
                "EXCEL" => await GenerateExcelReport(data),
                "CSV" => await GenerateCsvReport(data),
                _ => throw new ArgumentException("Unsupported format")
            };
        }

        private async Task<byte[]> GeneratePdfReport(List<Dictionary<string, object>> data)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document();
            var writer = PdfWriter.GetInstance(document, memoryStream);
            
            document.Open();
            document.Add(new Paragraph("Attendance Report"));
            document.Add(new Paragraph($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"));
            document.Add(new Paragraph(" "));

            if (data.Any())
            {
                var table = new PdfPTable(data.First().Keys.Count);
                
                // Add headers
                foreach (var key in data.First().Keys)
                {
                    table.AddCell(new PdfPCell(new Phrase(key)));
                }

                // Add data rows
                foreach (var row in data)
                {
                    foreach (var value in row.Values)
                    {
                        table.AddCell(new PdfPCell(new Phrase(value?.ToString() ?? "")));
                    }
                }

                document.Add(table);
            }

            document.Close();
            return memoryStream.ToArray();
        }

        private async Task<byte[]> GenerateExcelReport(List<Dictionary<string, object>> data)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Report");

            if (data.Any())
            {
                var headers = data.First().Keys.ToArray();
                
                // Add headers
                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                }

                // Add data
                for (int row = 0; row < data.Count; row++)
                {
                    var rowData = data[row];
                    for (int col = 0; col < headers.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = rowData[headers[col]];
                    }
                }

                worksheet.Cells.AutoFitColumns();
            }

            return package.GetAsByteArray();
        }

        private async Task<byte[]> GenerateCsvReport(List<Dictionary<string, object>> data)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);

            if (data.Any())
            {
                var headers = data.First().Keys.ToArray();
                
                // Write headers
                await writer.WriteLineAsync(string.Join(",", headers));

                // Write data
                foreach (var row in data)
                {
                    var values = headers.Select(h => EscapeCsvValue(row[h]?.ToString() ?? ""));
                    await writer.WriteLineAsync(string.Join(",", values));
                }
            }

            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        private string EscapeCsvValue(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        private async Task SendReportToRecipients(byte[] reportBytes, string reportName, string format, List<string> recipients)
        {
            // Implementation would send the report via email or other delivery method
            // For now, this is a placeholder
            _logger.LogInformation("Sending report {ReportName} to {RecipientCount} recipients", reportName, recipients.Count);
        }

        private DateTime? CalculateNextExecution(string cronExpression)
        {
            // Simplified cron calculation - in a real implementation, use a proper cron library
            return DateTime.UtcNow.AddDays(1);
        }

        private ScheduledReportDto MapToScheduledReportDto(ScheduledReport report)
        {
            return new ScheduledReportDto
            {
                Id = report.Id,
                TenantId = report.TenantId,
                ReportId = report.ReportId,
                Name = report.Name,
                CronExpression = report.CronExpression,
                Recipients = JsonSerializer.Deserialize<List<string>>(report.Recipients),
                Format = report.Format,
                Parameters = JsonSerializer.Deserialize<Dictionary<string, object>>(report.Parameters),
                IsActive = report.IsActive,
                LastExecuted = report.LastExecuted,
                NextExecution = report.NextExecution,
                CreatedAt = report.CreatedAt
            };
        }
    }

    public class DataExportService : IDataExportService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<DataExportService> _logger;

        public DataExportService(AttendancePlatformDbContext context, ILogger<DataExportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<byte[]> ExportToExcelAsync(ExportRequestDto request)
        {
            try
            {
                var data = await GetExportData(request);
                return await GenerateExcelFile(data, request.Options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to Excel");
                throw;
            }
        }

        public async Task<byte[]> ExportToPdfAsync(ExportRequestDto request)
        {
            try
            {
                var data = await GetExportData(request);
                return await GeneratePdfFile(data, request.Options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to PDF");
                throw;
            }
        }

        public async Task<byte[]> ExportToCsvAsync(ExportRequestDto request)
        {
            try
            {
                var data = await GetExportData(request);
                return await GenerateCsvFile(data, request.Options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to CSV");
                throw;
            }
        }

        public async Task<string> ExportToJsonAsync(ExportRequestDto request)
        {
            try
            {
                var data = await GetExportData(request);
                return JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting to JSON");
                throw;
            }
        }

        public async Task<ExportJobDto> CreateExportJobAsync(ExportRequestDto request)
        {
            try
            {
                var job = new ExportJob
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    Status = "queued",
                    Progress = 0,
                    CreatedAt = DateTime.UtcNow,
                    Parameters = JsonSerializer.Serialize(request)
                };

                _context.ExportJobs.Add(job);
                await _context.SaveChangesAsync();

                // In a real implementation, this would queue the job for background processing
                _ = Task.Run(() => ProcessExportJobAsync(job.Id));

                return MapToExportJobDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating export job");
                throw;
            }
        }

        public async Task<ExportJobDto> GetExportJobStatusAsync(Guid jobId)
        {
            try
            {
                var job = await _context.ExportJobs.FindAsync(jobId);
                if (job == null)
                    throw new ArgumentException("Export job not found");

                return MapToExportJobDto(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting export job status");
                throw;
            }
        }

        private async Task<List<Dictionary<string, object>>> GetExportData(ExportRequestDto request)
        {
            var results = new List<Dictionary<string, object>>();

            if (request.DataSource == "AttendanceRecords")
            {
                var query = _context.AttendanceRecords.AsQueryable();

                // Apply filters
                foreach (var filter in request.Filters)
                {
                    query = ApplyFilter(query, filter.Key, filter.Value);
                }

                var data = await query.Include(ar => ar.User).ToListAsync();

                foreach (var record in data)
                {
                    var row = new Dictionary<string, object>();
                    
                    foreach (var column in request.Columns)
                    {
                        var value = GetPropertyValue(record, column);
                        row[column] = value;
                    }

                    results.Add(row);
                }
            }

            return results;
        }

        private IQueryable<AttendanceRecord> ApplyFilter(IQueryable<AttendanceRecord> query, string filterKey, object filterValue)
        {
            return filterKey switch
            {
                "dateRange" => ApplyDateRangeFilter(query, filterValue.ToString()),
                "department" => query.Where(ar => ar.User.Department == filterValue.ToString()),
                "userId" => query.Where(ar => ar.UserId == Guid.Parse(filterValue.ToString())),
                _ => query
            };
        }

        private IQueryable<AttendanceRecord> ApplyDateRangeFilter(IQueryable<AttendanceRecord> query, string dateRange)
        {
            var (start, end) = ParseDateRange(dateRange);
            return query.Where(ar => ar.CheckInTime >= start && ar.CheckInTime <= end);
        }

        private (DateTime Start, DateTime End) ParseDateRange(string dateRange)
        {
            var now = DateTime.UtcNow;
            return dateRange switch
            {
                "today" => (now.Date, now.Date.AddDays(1).AddTicks(-1)),
                "week" => (now.Date.AddDays(-(int)now.DayOfWeek), now.Date.AddDays(7 - (int)now.DayOfWeek).AddTicks(-1)),
                "month" => (new DateTime(now.Year, now.Month, 1), new DateTime(now.Year, now.Month, 1).AddMonths(1).AddTicks(-1)),
                _ => (now.Date.AddDays(-30), now.Date.AddDays(1).AddTicks(-1))
            };
        }

        private object GetPropertyValue(object obj, string propertyName)
        {
            var type = obj.GetType();
            var property = type.GetProperty(propertyName);
            return property?.GetValue(obj) ?? "";
        }

        private async Task<byte[]> GenerateExcelFile(List<Dictionary<string, object>> data, Dictionary<string, object> options)
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Export");

            if (data.Any())
            {
                var headers = data.First().Keys.ToArray();
                
                // Add headers with styling
                for (int i = 0; i < headers.Length; i++)
                {
                    var cell = worksheet.Cells[1, i + 1];
                    cell.Value = headers[i];
                    cell.Style.Font.Bold = true;
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Add data
                for (int row = 0; row < data.Count; row++)
                {
                    var rowData = data[row];
                    for (int col = 0; col < headers.Length; col++)
                    {
                        worksheet.Cells[row + 2, col + 1].Value = rowData[headers[col]];
                    }
                }

                worksheet.Cells.AutoFitColumns();
            }

            return package.GetAsByteArray();
        }

        private async Task<byte[]> GeneratePdfFile(List<Dictionary<string, object>> data, Dictionary<string, object> options)
        {
            using var memoryStream = new MemoryStream();
            var document = new Document(PageSize.A4.Rotate());
            var writer = PdfWriter.GetInstance(document, memoryStream);
            
            document.Open();
            document.Add(new Paragraph("Data Export"));
            document.Add(new Paragraph($"Generated on: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}"));
            document.Add(new Paragraph(" "));

            if (data.Any())
            {
                var headers = data.First().Keys.ToArray();
                var table = new PdfPTable(headers.Length);
                
                // Add headers
                foreach (var header in headers)
                {
                    var cell = new PdfPCell(new Phrase(header));
                    cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    table.AddCell(cell);
                }

                // Add data rows
                foreach (var row in data)
                {
                    foreach (var header in headers)
                    {
                        table.AddCell(new PdfPCell(new Phrase(row[header]?.ToString() ?? "")));
                    }
                }

                document.Add(table);
            }

            document.Close();
            return memoryStream.ToArray();
        }

        private async Task<byte[]> GenerateCsvFile(List<Dictionary<string, object>> data, Dictionary<string, object> options)
        {
            using var memoryStream = new MemoryStream();
            using var writer = new StreamWriter(memoryStream);

            if (data.Any())
            {
                var headers = data.First().Keys.ToArray();
                
                // Write headers
                await writer.WriteLineAsync(string.Join(",", headers));

                // Write data
                foreach (var row in data)
                {
                    var values = headers.Select(h => EscapeCsvValue(row[h]?.ToString() ?? ""));
                    await writer.WriteLineAsync(string.Join(",", values));
                }
            }

            await writer.FlushAsync();
            return memoryStream.ToArray();
        }

        private string EscapeCsvValue(string value)
        {
            if (value.Contains(",") || value.Contains("\"") || value.Contains("\n"))
            {
                return $"\"{value.Replace("\"", "\"\"")}\"";
            }
            return value;
        }

        private async Task ProcessExportJobAsync(Guid jobId)
        {
            try
            {
                var job = await _context.ExportJobs.FindAsync(jobId);
                if (job == null) return;

                job.Status = "processing";
                job.Progress = 10;
                await _context.SaveChangesAsync();

                var request = JsonSerializer.Deserialize<ExportRequestDto>(job.Parameters);
                
                job.Progress = 50;
                await _context.SaveChangesAsync();

                var data = await GetExportData(request);
                
                job.Progress = 80;
                await _context.SaveChangesAsync();

                var fileBytes = request.Format.ToUpper() switch
                {
                    "EXCEL" => await GenerateExcelFile(data, request.Options),
                    "PDF" => await GeneratePdfFile(data, request.Options),
                    "CSV" => await GenerateCsvFile(data, request.Options),
                    _ => throw new ArgumentException("Unsupported format")
                };

                // In a real implementation, save the file and provide download URL
                job.Status = "completed";
                job.Progress = 100;
                job.CompletedAt = DateTime.UtcNow;
                job.DownloadUrl = $"/api/exports/{jobId}/download";
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var job = await _context.ExportJobs.FindAsync(jobId);
                if (job != null)
                {
                    job.Status = "failed";
                    job.ErrorMessage = ex.Message;
                    await _context.SaveChangesAsync();
                }
                
                _logger.LogError(ex, "Error processing export job {JobId}", jobId);
            }
        }

        private ExportJobDto MapToExportJobDto(ExportJob job)
        {
            return new ExportJobDto
            {
                Id = job.Id,
                Status = job.Status,
                Progress = job.Progress,
                DownloadUrl = job.DownloadUrl,
                CreatedAt = job.CreatedAt,
                CompletedAt = job.CompletedAt,
                ErrorMessage = job.ErrorMessage
            };
        }
    }

    // Additional entity models
    public class ScheduledReport
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid ReportId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string CronExpression { get; set; } = string.Empty;
        public string Recipients { get; set; } = string.Empty;
        public string Format { get; set; } = string.Empty;
        public string Parameters { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastExecuted { get; set; }
        public DateTime? NextExecution { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ExportJob
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Progress { get; set; }
        public string? DownloadUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? ErrorMessage { get; set; }
        public string Parameters { get; set; } = string.Empty;
    }
}

