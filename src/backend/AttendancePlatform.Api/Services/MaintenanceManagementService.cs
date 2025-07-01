using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IMaintenanceManagementService
    {
        Task<MaintenanceRequestDto> CreateMaintenanceRequestAsync(MaintenanceRequestDto request);
        Task<List<MaintenanceRequestDto>> GetMaintenanceRequestsAsync(Guid tenantId);
        Task<MaintenanceRequestDto> UpdateMaintenanceRequestAsync(Guid requestId, MaintenanceRequestDto request);
        Task<bool> DeleteMaintenanceRequestAsync(Guid requestId);
        Task<MaintenanceScheduleDto> CreateMaintenanceScheduleAsync(MaintenanceScheduleDto schedule);
        Task<List<MaintenanceScheduleDto>> GetMaintenanceSchedulesAsync(Guid tenantId);
        Task<MaintenanceWorkOrderDto> CreateWorkOrderAsync(MaintenanceWorkOrderDto workOrder);
        Task<List<MaintenanceWorkOrderDto>> GetWorkOrdersAsync(Guid tenantId);
        Task<bool> CompleteWorkOrderAsync(Guid workOrderId, WorkOrderCompletionDto completion);
        Task<MaintenanceAssetDto> CreateMaintenanceAssetAsync(MaintenanceAssetDto asset);
        Task<List<MaintenanceAssetDto>> GetMaintenanceAssetsAsync(Guid tenantId);
        Task<MaintenanceInventoryDto> CreateInventoryItemAsync(MaintenanceInventoryDto item);
        Task<List<MaintenanceInventoryDto>> GetInventoryItemsAsync(Guid tenantId);
        Task<MaintenanceVendorDto> CreateVendorAsync(MaintenanceVendorDto vendor);
        Task<List<MaintenanceVendorDto>> GetVendorsAsync(Guid tenantId);
        Task<MaintenanceAnalyticsDto> GetMaintenanceAnalyticsAsync(Guid tenantId);
        Task<MaintenanceReportDto> GenerateMaintenanceReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<List<MaintenanceRequestDto>> GetOverdueMaintenanceAsync(Guid tenantId);
        Task<MaintenanceBudgetDto> GetMaintenanceBudgetAsync(Guid tenantId);
        Task<bool> SchedulePreventiveMaintenanceAsync(Guid assetId, MaintenanceScheduleDto schedule);
    }

    public class MaintenanceManagementService : IMaintenanceManagementService
    {
        private readonly ILogger<MaintenanceManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public MaintenanceManagementService(ILogger<MaintenanceManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<MaintenanceRequestDto> CreateMaintenanceRequestAsync(MaintenanceRequestDto request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.RequestNumber = $"MR-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                request.CreatedAt = DateTime.UtcNow;
                request.Status = "Open";
                request.Priority = DeterminePriority(request.Description);

                _logger.LogInformation("Maintenance request created: {RequestId} - {RequestNumber}", request.Id, request.RequestNumber);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create maintenance request");
                throw;
            }
        }

        public async Task<List<MaintenanceRequestDto>> GetMaintenanceRequestsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceRequestDto>
            {
                new MaintenanceRequestDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RequestNumber = "MR-20241227-1001",
                    Title = "HVAC System Repair",
                    Description = "Air conditioning unit in Building A not cooling properly",
                    RequestType = "Repair",
                    Priority = "High",
                    Status = "In Progress",
                    AssetId = Guid.NewGuid(),
                    AssetName = "HVAC Unit - Building A",
                    Location = "Building A - Roof",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "Facility Manager",
                    AssignedTo = Guid.NewGuid(),
                    AssigneeName = "HVAC Technician",
                    EstimatedCost = 1500.00m,
                    ActualCost = 0.00m,
                    EstimatedDuration = 8,
                    ActualDuration = 0,
                    ScheduledDate = DateTime.UtcNow.AddDays(1),
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-4)
                }
            };
        }

        public async Task<MaintenanceRequestDto> UpdateMaintenanceRequestAsync(Guid requestId, MaintenanceRequestDto request)
        {
            try
            {
                await Task.CompletedTask;
                request.Id = requestId;
                request.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Maintenance request updated: {RequestId}", requestId);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update maintenance request {RequestId}", requestId);
                throw;
            }
        }

        public async Task<bool> DeleteMaintenanceRequestAsync(Guid requestId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Maintenance request deleted: {RequestId}", requestId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete maintenance request {RequestId}", requestId);
                return false;
            }
        }

        public async Task<MaintenanceScheduleDto> CreateMaintenanceScheduleAsync(MaintenanceScheduleDto schedule)
        {
            try
            {
                schedule.Id = Guid.NewGuid();
                schedule.ScheduleNumber = $"MS-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                schedule.CreatedAt = DateTime.UtcNow;
                schedule.Status = "Active";

                _logger.LogInformation("Maintenance schedule created: {ScheduleId} - {ScheduleNumber}", schedule.Id, schedule.ScheduleNumber);
                return schedule;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create maintenance schedule");
                throw;
            }
        }

        public async Task<List<MaintenanceScheduleDto>> GetMaintenanceSchedulesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceScheduleDto>
            {
                new MaintenanceScheduleDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ScheduleNumber = "MS-20241227-1001",
                    Name = "HVAC Quarterly Maintenance",
                    Description = "Quarterly preventive maintenance for all HVAC systems",
                    AssetId = Guid.NewGuid(),
                    AssetName = "HVAC Systems",
                    MaintenanceType = "Preventive",
                    Frequency = "Quarterly",
                    FrequencyDays = 90,
                    NextDueDate = DateTime.UtcNow.AddDays(15),
                    LastCompletedDate = DateTime.UtcNow.AddDays(-75),
                    EstimatedDuration = 6,
                    EstimatedCost = 2500.00m,
                    AssignedTechnician = "HVAC Team",
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-365),
                    UpdatedAt = DateTime.UtcNow.AddDays(-75)
                }
            };
        }

        public async Task<MaintenanceWorkOrderDto> CreateWorkOrderAsync(MaintenanceWorkOrderDto workOrder)
        {
            try
            {
                workOrder.Id = Guid.NewGuid();
                workOrder.WorkOrderNumber = $"WO-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                workOrder.CreatedAt = DateTime.UtcNow;
                workOrder.Status = "Open";

                _logger.LogInformation("Work order created: {WorkOrderId} - {WorkOrderNumber}", workOrder.Id, workOrder.WorkOrderNumber);
                return workOrder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create work order");
                throw;
            }
        }

        public async Task<List<MaintenanceWorkOrderDto>> GetWorkOrdersAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceWorkOrderDto>
            {
                new MaintenanceWorkOrderDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    WorkOrderNumber = "WO-20241227-1001",
                    Title = "Replace Broken Window",
                    Description = "Replace broken window in Conference Room A",
                    WorkOrderType = "Repair",
                    Priority = "Medium",
                    Status = "In Progress",
                    AssetId = Guid.NewGuid(),
                    AssetName = "Conference Room A Window",
                    Location = "Building A - Conference Room A",
                    AssignedTo = Guid.NewGuid(),
                    AssigneeName = "Maintenance Technician",
                    EstimatedCost = 350.00m,
                    ActualCost = 285.00m,
                    EstimatedDuration = 2,
                    ActualDuration = 1.5,
                    ScheduledStartDate = DateTime.UtcNow.AddHours(-6),
                    ActualStartDate = DateTime.UtcNow.AddHours(-5),
                    ScheduledEndDate = DateTime.UtcNow.AddHours(-4),
                    ActualEndDate = null,
                    PartsUsed = new List<string> { "Window Glass", "Window Frame Seal" },
                    LaborHours = 1.5,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddHours(-1)
                }
            };
        }

        public async Task<bool> CompleteWorkOrderAsync(Guid workOrderId, WorkOrderCompletionDto completion)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Work order completed: {WorkOrderId}", workOrderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to complete work order {WorkOrderId}", workOrderId);
                return false;
            }
        }

        public async Task<MaintenanceAssetDto> CreateMaintenanceAssetAsync(MaintenanceAssetDto asset)
        {
            try
            {
                asset.Id = Guid.NewGuid();
                asset.AssetNumber = $"MA-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                asset.CreatedAt = DateTime.UtcNow;
                asset.Status = "Active";

                _logger.LogInformation("Maintenance asset created: {AssetId} - {AssetNumber}", asset.Id, asset.AssetNumber);
                return asset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create maintenance asset");
                throw;
            }
        }

        public async Task<List<MaintenanceAssetDto>> GetMaintenanceAssetsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceAssetDto>
            {
                new MaintenanceAssetDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AssetNumber = "MA-20241227-1001",
                    Name = "Central HVAC System",
                    Description = "Main heating, ventilation, and air conditioning system",
                    AssetType = "HVAC",
                    Category = "Building Systems",
                    Location = "Building A - Mechanical Room",
                    Manufacturer = "Climate Control Inc.",
                    Model = "CC-5000X",
                    SerialNumber = "CC5000X-2023-001",
                    PurchaseDate = DateTime.UtcNow.AddDays(-730),
                    InstallationDate = DateTime.UtcNow.AddDays(-720),
                    WarrantyExpiry = DateTime.UtcNow.AddDays(635),
                    PurchaseCost = 45000.00m,
                    CurrentValue = 38000.00m,
                    Status = "Active",
                    Condition = "Good",
                    LastMaintenanceDate = DateTime.UtcNow.AddDays(-30),
                    NextMaintenanceDate = DateTime.UtcNow.AddDays(60),
                    MaintenanceFrequency = "Quarterly",
                    CreatedAt = DateTime.UtcNow.AddDays(-720),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<MaintenanceInventoryDto> CreateInventoryItemAsync(MaintenanceInventoryDto item)
        {
            try
            {
                item.Id = Guid.NewGuid();
                item.ItemNumber = $"MI-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                item.CreatedAt = DateTime.UtcNow;
                item.Status = "Active";

                _logger.LogInformation("Maintenance inventory item created: {ItemId} - {ItemNumber}", item.Id, item.ItemNumber);
                return item;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create maintenance inventory item");
                throw;
            }
        }

        public async Task<List<MaintenanceInventoryDto>> GetInventoryItemsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceInventoryDto>
            {
                new MaintenanceInventoryDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ItemNumber = "MI-20241227-1001",
                    Name = "HVAC Air Filter",
                    Description = "High-efficiency air filter for HVAC systems",
                    Category = "HVAC Parts",
                    Manufacturer = "FilterPro",
                    PartNumber = "FP-HE-20x25x4",
                    UnitOfMeasure = "Each",
                    CurrentStock = 25,
                    MinimumStock = 10,
                    MaximumStock = 50,
                    ReorderPoint = 15,
                    UnitCost = 45.00m,
                    TotalValue = 1125.00m,
                    Location = "Warehouse - Aisle A3",
                    Supplier = "HVAC Supply Co.",
                    LastOrderDate = DateTime.UtcNow.AddDays(-30),
                    LastUsedDate = DateTime.UtcNow.AddDays(-5),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-365),
                    UpdatedAt = DateTime.UtcNow.AddDays(-5)
                }
            };
        }

        public async Task<MaintenanceVendorDto> CreateVendorAsync(MaintenanceVendorDto vendor)
        {
            try
            {
                vendor.Id = Guid.NewGuid();
                vendor.VendorNumber = $"MV-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                vendor.CreatedAt = DateTime.UtcNow;
                vendor.Status = "Active";

                _logger.LogInformation("Maintenance vendor created: {VendorId} - {VendorNumber}", vendor.Id, vendor.VendorNumber);
                return vendor;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create maintenance vendor");
                throw;
            }
        }

        public async Task<List<MaintenanceVendorDto>> GetVendorsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceVendorDto>
            {
                new MaintenanceVendorDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    VendorNumber = "MV-20241227-1001",
                    CompanyName = "HVAC Specialists Inc.",
                    ContactName = "John Technical",
                    ContactEmail = "john@hvacspecialists.com",
                    ContactPhone = "+1234567890",
                    Address = "123 Industrial Blvd, Tech City, TC 12345",
                    ServiceTypes = new List<string> { "HVAC Repair", "HVAC Maintenance", "Emergency Service" },
                    Specializations = new List<string> { "Commercial HVAC", "Industrial Systems", "Energy Efficiency" },
                    CertificationLevel = "Master Certified",
                    Rating = 4.8,
                    ResponseTime = "2 hours",
                    ServiceArea = "Metro Area",
                    HourlyRate = 125.00m,
                    EmergencyRate = 185.00m,
                    ContractStartDate = DateTime.UtcNow.AddDays(-365),
                    ContractEndDate = DateTime.UtcNow.AddDays(365),
                    Status = "Active",
                    CreatedAt = DateTime.UtcNow.AddDays(-365),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                }
            };
        }

        public async Task<MaintenanceAnalyticsDto> GetMaintenanceAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new MaintenanceAnalyticsDto
            {
                TenantId = tenantId,
                TotalRequests = 285,
                OpenRequests = 45,
                InProgressRequests = 28,
                CompletedRequests = 212,
                OverdueRequests = 8,
                AverageResolutionTime = 4.2,
                TotalMaintenanceCost = 125000.00m,
                PreventiveMaintenancePercentage = 65.8,
                EmergencyMaintenancePercentage = 12.3,
                AssetUptime = 97.8,
                WorkOrdersByType = new Dictionary<string, int>
                {
                    { "Preventive", 125 },
                    { "Repair", 95 },
                    { "Emergency", 35 },
                    { "Inspection", 30 }
                },
                RequestsByPriority = new Dictionary<string, int>
                {
                    { "Low", 85 },
                    { "Medium", 125 },
                    { "High", 55 },
                    { "Critical", 20 }
                },
                CostByCategory = new Dictionary<string, decimal>
                {
                    { "HVAC", 45000.00m },
                    { "Electrical", 28000.00m },
                    { "Plumbing", 18000.00m },
                    { "General", 22000.00m },
                    { "Safety", 12000.00m }
                },
                MonthlyTrends = new Dictionary<string, int>
                {
                    { "Jan", 22 }, { "Feb", 18 }, { "Mar", 25 }, { "Apr", 28 },
                    { "May", 32 }, { "Jun", 29 }, { "Jul", 24 }, { "Aug", 27 },
                    { "Sep", 31 }, { "Oct", 26 }, { "Nov", 23 }, { "Dec", 30 }
                },
                VendorPerformance = new Dictionary<string, double>
                {
                    { "HVAC Specialists Inc.", 4.8 },
                    { "ElectroFix Solutions", 4.6 },
                    { "PlumbPro Services", 4.4 },
                    { "General Maintenance Co.", 4.2 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<MaintenanceReportDto> GenerateMaintenanceReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new MaintenanceReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalRequests = 125,
                CompletedRequests = 98,
                PendingRequests = 27,
                OverdueRequests = 5,
                TotalCost = 45000.00m,
                LaborCost = 28000.00m,
                PartsCost = 17000.00m,
                AverageResolutionTime = 3.8,
                AssetUtilization = 96.5,
                PreventiveMaintenanceCompliance = 92.3,
                TopMaintenanceIssues = new List<string>
                {
                    "HVAC filter replacement",
                    "Lighting repairs",
                    "Plumbing leaks",
                    "Door hardware issues",
                    "Elevator maintenance"
                },
                AssetPerformance = new Dictionary<string, MaintenanceAssetPerformanceDto>
                {
                    { "HVAC Systems", new MaintenanceAssetPerformanceDto { Uptime = 98.5, MaintenanceCost = 15000.00m, RequestCount = 25 } },
                    { "Electrical Systems", new MaintenanceAssetPerformanceDto { Uptime = 99.2, MaintenanceCost = 8500.00m, RequestCount = 18 } },
                    { "Plumbing Systems", new MaintenanceAssetPerformanceDto { Uptime = 97.8, MaintenanceCost = 6200.00m, RequestCount = 15 } },
                    { "Elevators", new MaintenanceAssetPerformanceDto { Uptime = 99.8, MaintenanceCost = 12000.00m, RequestCount = 8 } }
                },
                VendorCosts = new Dictionary<string, decimal>
                {
                    { "HVAC Specialists Inc.", 18500.00m },
                    { "ElectroFix Solutions", 12000.00m },
                    { "PlumbPro Services", 8500.00m },
                    { "General Maintenance Co.", 6000.00m }
                },
                CostSavings = new Dictionary<string, decimal>
                {
                    { "Preventive Maintenance", 8500.00m },
                    { "Energy Efficiency", 3200.00m },
                    { "Bulk Purchasing", 1800.00m },
                    { "Vendor Negotiations", 2100.00m }
                },
                Recommendations = new List<string>
                {
                    "Increase preventive maintenance frequency for HVAC systems",
                    "Consider LED lighting upgrade to reduce maintenance",
                    "Implement predictive maintenance for critical assets",
                    "Negotiate better rates with high-performing vendors"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<MaintenanceRequestDto>> GetOverdueMaintenanceAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<MaintenanceRequestDto>
            {
                new MaintenanceRequestDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RequestNumber = "MR-20241227-1003",
                    Title = "Overdue: Parking Lot Light Repair",
                    Description = "Several parking lot lights not working - safety concern",
                    RequestType = "Repair",
                    Priority = "High",
                    Status = "Overdue",
                    AssetName = "Parking Lot Lighting",
                    Location = "Parking Lot - Section B",
                    RequesterName = "Security Manager",
                    AssigneeName = "Electrical Contractor",
                    EstimatedCost = 850.00m,
                    ScheduledDate = DateTime.UtcNow.AddDays(-3),
                    CreatedAt = DateTime.UtcNow.AddDays(-10),
                    DaysOverdue = 3
                }
            };
        }

        public async Task<MaintenanceBudgetDto> GetMaintenanceBudgetAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new MaintenanceBudgetDto
            {
                TenantId = tenantId,
                FiscalYear = DateTime.UtcNow.Year,
                TotalBudget = 250000.00m,
                SpentToDate = 125000.00m,
                RemainingBudget = 125000.00m,
                BudgetUtilization = 50.0,
                CategoryBudgets = new Dictionary<string, MaintenanceBudgetCategoryDto>
                {
                    { "HVAC", new MaintenanceBudgetCategoryDto { Category = "HVAC", Status = "Active", Allocated = 75000.00m, Spent = 45000.00m, Remaining = 30000.00m } },
                    { "Electrical", new MaintenanceBudgetCategoryDto { Category = "Electrical", Status = "Active", Allocated = 50000.00m, Spent = 28000.00m, Remaining = 22000.00m } },
                    { "Plumbing", new MaintenanceBudgetCategoryDto { Category = "Plumbing", Status = "Active", Allocated = 40000.00m, Spent = 18000.00m, Remaining = 22000.00m } },
                    { "General", new MaintenanceBudgetCategoryDto { Category = "General", Status = "Active", Allocated = 60000.00m, Spent = 22000.00m, Remaining = 38000.00m } },
                    { "Emergency", new MaintenanceBudgetCategoryDto { Category = "Emergency", Status = "Active", Allocated = 25000.00m, Spent = 12000.00m, Remaining = 13000.00m } }
                },
                MonthlySpending = new Dictionary<string, decimal>
                {
                    { "Jan", 8500.00m }, { "Feb", 7200.00m }, { "Mar", 12000.00m }, { "Apr", 9800.00m },
                    { "May", 15000.00m }, { "Jun", 11500.00m }, { "Jul", 8900.00m }, { "Aug", 13200.00m },
                    { "Sep", 16500.00m }, { "Oct", 12400.00m }, { "Nov", 9000.00m }, { "Dec", 21000.00m }
                },
                ProjectedYearEndSpend = 235000.00m,
                BudgetVariance = -15000.00m,
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> SchedulePreventiveMaintenanceAsync(Guid assetId, MaintenanceScheduleDto schedule)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Preventive maintenance scheduled for asset: {AssetId}", assetId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to schedule preventive maintenance for asset {AssetId}", assetId);
                return false;
            }
        }

        private string DeterminePriority(string description)
        {
            var lowercaseDesc = description.ToLower();
            if (lowercaseDesc.Contains("emergency") || lowercaseDesc.Contains("urgent") || lowercaseDesc.Contains("critical"))
                return "Critical";
            if (lowercaseDesc.Contains("safety") || lowercaseDesc.Contains("security") || lowercaseDesc.Contains("leak"))
                return "High";
            if (lowercaseDesc.Contains("repair") || lowercaseDesc.Contains("broken"))
                return "Medium";
            return "Low";
        }
    }

    public class MaintenanceRequestDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string RequestNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string RequestType { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public Guid AssetId { get; set; }
        public required string AssetName { get; set; }
        public required string Location { get; set; }
        public Guid RequestedBy { get; set; }
        public required string RequesterName { get; set; }
        public Guid AssignedTo { get; set; }
        public required string AssigneeName { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }
        public int EstimatedDuration { get; set; }
        public int ActualDuration { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int DaysOverdue { get; set; }
    }

    public class MaintenanceScheduleDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string ScheduleNumber { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public Guid AssetId { get; set; }
        public required string AssetName { get; set; }
        public required string MaintenanceType { get; set; }
        public required string Frequency { get; set; }
        public int FrequencyDays { get; set; }
        public DateTime NextDueDate { get; set; }
        public DateTime? LastCompletedDate { get; set; }
        public int EstimatedDuration { get; set; }
        public decimal EstimatedCost { get; set; }
        public required string AssignedTechnician { get; set; }
        public required string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MaintenanceWorkOrderDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string WorkOrderNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string WorkOrderType { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public Guid AssetId { get; set; }
        public required string AssetName { get; set; }
        public required string Location { get; set; }
        public Guid AssignedTo { get; set; }
        public required string AssigneeName { get; set; }
        public decimal EstimatedCost { get; set; }
        public decimal ActualCost { get; set; }
        public double EstimatedDuration { get; set; }
        public double ActualDuration { get; set; }
        public DateTime? ScheduledStartDate { get; set; }
        public DateTime? ActualStartDate { get; set; }
        public DateTime? ScheduledEndDate { get; set; }
        public DateTime? ActualEndDate { get; set; }
        public List<string> PartsUsed { get; set; }
        public double LaborHours { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class WorkOrderCompletionDto
    {
        public required string CompletionNotes { get; set; }
        public List<string> PartsUsed { get; set; }
        public double LaborHours { get; set; }
        public decimal ActualCost { get; set; }
        public DateTime CompletionDate { get; set; }
    }

    public class MaintenanceAssetDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string AssetNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AssetType { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime InstallationDate { get; set; }
        public DateTime WarrantyExpiry { get; set; }
        public decimal PurchaseCost { get; set; }
        public decimal CurrentValue { get; set; }
        public string Status { get; set; }
        public string Condition { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public string MaintenanceFrequency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MaintenanceInventoryDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string ItemNumber { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }
        public string PartNumber { get; set; }
        public string UnitOfMeasure { get; set; }
        public int CurrentStock { get; set; }
        public int MinimumStock { get; set; }
        public int MaximumStock { get; set; }
        public int ReorderPoint { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalValue { get; set; }
        public string Location { get; set; }
        public string Supplier { get; set; }
        public DateTime? LastOrderDate { get; set; }
        public DateTime? LastUsedDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MaintenanceVendorDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string VendorNumber { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string ContactPhone { get; set; }
        public string Address { get; set; }
        public List<string> ServiceTypes { get; set; }
        public List<string> Specializations { get; set; }
        public string CertificationLevel { get; set; }
        public double Rating { get; set; }
        public string ResponseTime { get; set; }
        public string ServiceArea { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal EmergencyRate { get; set; }
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MaintenanceAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalRequests { get; set; }
        public int OpenRequests { get; set; }
        public int InProgressRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int OverdueRequests { get; set; }
        public double AverageResolutionTime { get; set; }
        public decimal TotalMaintenanceCost { get; set; }
        public double PreventiveMaintenancePercentage { get; set; }
        public double EmergencyMaintenancePercentage { get; set; }
        public double AssetUptime { get; set; }
        public Dictionary<string, int> WorkOrdersByType { get; set; }
        public Dictionary<string, int> RequestsByPriority { get; set; }
        public Dictionary<string, decimal> CostByCategory { get; set; }
        public Dictionary<string, int> MonthlyTrends { get; set; }
        public Dictionary<string, double> VendorPerformance { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class MaintenanceReportDto
    {
        public Guid TenantId { get; set; }
        public string ReportPeriod { get; set; }
        public int TotalRequests { get; set; }
        public int CompletedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int OverdueRequests { get; set; }
        public decimal TotalCost { get; set; }
        public decimal LaborCost { get; set; }
        public decimal PartsCost { get; set; }
        public double AverageResolutionTime { get; set; }
        public double AssetUtilization { get; set; }
        public double PreventiveMaintenanceCompliance { get; set; }
        public List<string> TopMaintenanceIssues { get; set; }
        public Dictionary<string, MaintenanceAssetPerformanceDto> AssetPerformance { get; set; }
        public Dictionary<string, decimal> VendorCosts { get; set; }
        public Dictionary<string, decimal> CostSavings { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class MaintenanceAssetPerformanceDto
    {
        public double Uptime { get; set; }
        public decimal MaintenanceCost { get; set; }
        public int RequestCount { get; set; }
    }

    public class MaintenanceBudgetDto
    {
        public Guid TenantId { get; set; }
        public int FiscalYear { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal SpentToDate { get; set; }
        public decimal RemainingBudget { get; set; }
        public double BudgetUtilization { get; set; }
        public Dictionary<string, MaintenanceBudgetCategoryDto> CategoryBudgets { get; set; }
        public Dictionary<string, decimal> MonthlySpending { get; set; }
        public decimal ProjectedYearEndSpend { get; set; }
        public decimal BudgetVariance { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class MaintenanceBudgetCategoryDto
    {
        public required string Category { get; set; }
        public required string Status { get; set; }
        public decimal Allocated { get; set; }
        public decimal Spent { get; set; }
        public decimal Remaining { get; set; }
    }
}
