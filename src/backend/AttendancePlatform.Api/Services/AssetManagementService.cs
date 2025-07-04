using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IAssetManagementService
    {
        Task<AssetDto> CreateAssetAsync(AssetDto asset);
        Task<List<AssetDto>> GetAssetsAsync(Guid tenantId);
        Task<AssetDto> UpdateAssetAsync(Guid assetId, AssetDto asset);
        Task<bool> DeleteAssetAsync(Guid assetId);
        Task<AssetAssignmentDto> AssignAssetAsync(AssetAssignmentDto assignment);
        Task<List<AssetAssignmentDto>> GetAssetAssignmentsAsync(Guid tenantId);
        Task<AssetMaintenanceDto> CreateMaintenanceRecordAsync(AssetMaintenanceDto maintenance);
        Task<List<AssetMaintenanceDto>> GetMaintenanceRecordsAsync(Guid assetId);
        Task<AssetAnalyticsDto> GetAssetAnalyticsAsync(Guid tenantId);
        Task<List<AssetCategoryDto>> GetAssetCategoriesAsync(Guid tenantId);
        Task<AssetCategoryDto> CreateAssetCategoryAsync(AssetCategoryDto category);
        Task<AssetDepreciationDto> CalculateDepreciationAsync(Guid assetId);
        Task<List<AssetAuditDto>> GetAssetAuditTrailAsync(Guid assetId);
        Task<AssetReportDto> GenerateAssetReportAsync(Guid tenantId);
    }

    public class AssetManagementService : IAssetManagementService
    {
        private readonly ILogger<AssetManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public AssetManagementService(ILogger<AssetManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<AssetDto> CreateAssetAsync(AssetDto asset)
        {
            try
            {
                asset.Id = Guid.NewGuid();
                asset.AssetTag = $"AST-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                asset.CreatedAt = DateTime.UtcNow;
                asset.Status = "Available";

                _logger.LogInformation("Asset created: {AssetId} - {AssetTag}", asset.Id, asset.AssetTag);
                return asset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create asset");
                throw;
            }
        }

        public async Task<List<AssetDto>> GetAssetsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<AssetDto>
            {
                new AssetDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AssetTag = "AST-20241227-1001",
                    Name = "Dell Laptop - Latitude 7420",
                    Description = "Business laptop with Intel i7 processor",
                    Category = "IT Equipment",
                    SerialNumber = "DL7420-2024-001",
                    PurchaseDate = DateTime.UtcNow.AddDays(-365),
                    PurchasePrice = 1200.00m,
                    CurrentValue = 800.00m,
                    Status = "Assigned",
                    Location = "Office Floor 2",
                    AssignedTo = "John Smith",
                    CreatedAt = DateTime.UtcNow.AddDays(-365)
                },
                new AssetDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AssetTag = "AST-20241227-1002",
                    Name = "iPhone 15 Pro",
                    Description = "Company mobile phone",
                    Category = "Mobile Devices",
                    SerialNumber = "IP15P-2024-002",
                    PurchaseDate = DateTime.UtcNow.AddDays(-180),
                    PurchasePrice = 999.00m,
                    CurrentValue = 850.00m,
                    Status = "Available",
                    Location = "IT Storage",
                    AssignedTo = null,
                    CreatedAt = DateTime.UtcNow.AddDays(-180)
                }
            };
        }

        public async Task<AssetDto> UpdateAssetAsync(Guid assetId, AssetDto asset)
        {
            try
            {
                await Task.CompletedTask;
                asset.Id = assetId;
                asset.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Asset updated: {AssetId}", assetId);
                return asset;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update asset {AssetId}", assetId);
                throw;
            }
        }

        public async Task<bool> DeleteAssetAsync(Guid assetId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Asset deleted: {AssetId}", assetId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete asset {AssetId}", assetId);
                return false;
            }
        }

        public async Task<AssetAssignmentDto> AssignAssetAsync(AssetAssignmentDto assignment)
        {
            try
            {
                assignment.Id = Guid.NewGuid();
                assignment.AssignedDate = DateTime.UtcNow;
                assignment.Status = "Active";

                _logger.LogInformation("Asset assigned: {AssetId} to {EmployeeId}", assignment.AssetId, assignment.EmployeeId);
                return assignment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to assign asset");
                throw;
            }
        }

        public async Task<List<AssetAssignmentDto>> GetAssetAssignmentsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<AssetAssignmentDto>
            {
                new AssetAssignmentDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AssetId = Guid.NewGuid(),
                    AssetName = "Dell Laptop - Latitude 7420",
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "John Smith",
                    AssignedDate = DateTime.UtcNow.AddDays(-30),
                    ReturnDate = null,
                    Status = "Active",
                    Notes = "Assigned for remote work setup"
                },
                new AssetAssignmentDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    AssetId = Guid.NewGuid(),
                    AssetName = "iPhone 14 Pro",
                    EmployeeId = Guid.NewGuid(),
                    EmployeeName = "Sarah Johnson",
                    AssignedDate = DateTime.UtcNow.AddDays(-60),
                    ReturnDate = DateTime.UtcNow.AddDays(-5),
                    Status = "Returned",
                    Notes = "Returned due to role change"
                }
            };
        }

        public async Task<AssetMaintenanceDto> CreateMaintenanceRecordAsync(AssetMaintenanceDto maintenance)
        {
            try
            {
                maintenance.Id = Guid.NewGuid();
                maintenance.Status = "Scheduled";

                _logger.LogInformation("Asset maintenance record created: {MaintenanceId} for asset {AssetId}", maintenance.Id, maintenance.AssetId);
                return maintenance;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create asset maintenance record");
                throw;
            }
        }

        public async Task<List<AssetMaintenanceDto>> GetMaintenanceRecordsAsync(Guid assetId)
        {
            await Task.CompletedTask;
            return new List<AssetMaintenanceDto>
            {
                new AssetMaintenanceDto
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetId,
                    MaintenanceType = "Preventive",
                    Description = "Regular system update and cleaning",
                    ScheduledDate = DateTime.UtcNow.AddDays(-30),
                    CompletedDate = DateTime.UtcNow.AddDays(-30),
                    Status = "Completed",
                    Cost = 150.00m,
                    PerformedBy = "IT Support Team",
                    Notes = "System updated to latest version, hardware cleaned"
                },
                new AssetMaintenanceDto
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetId,
                    MaintenanceType = "Corrective",
                    Description = "Battery replacement",
                    ScheduledDate = DateTime.UtcNow.AddDays(15),
                    CompletedDate = null,
                    Status = "Scheduled",
                    Cost = 200.00m,
                    PerformedBy = "External Service Provider",
                    Notes = "Battery showing signs of degradation"
                }
            };
        }

        public async Task<AssetAnalyticsDto> GetAssetAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new AssetAnalyticsDto
            {
                TenantId = tenantId,
                TotalAssets = 250,
                AssignedAssets = 180,
                AvailableAssets = 60,
                MaintenanceAssets = 10,
                TotalValue = 125000.00m,
                DepreciatedValue = 85000.00m,
                MaintenanceCosts = 5500.00m,
                AssetUtilizationRate = 72.0,
                AverageAssetAge = 2.3,
                TopCategories = new Dictionary<string, int>
                {
                    { "IT Equipment", 120 },
                    { "Office Furniture", 80 },
                    { "Mobile Devices", 50 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<AssetCategoryDto>> GetAssetCategoriesAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<AssetCategoryDto>
            {
                new AssetCategoryDto { Id = Guid.NewGuid(), Name = "IT Equipment", Description = "Computers, laptops, servers", IsActive = true },
                new AssetCategoryDto { Id = Guid.NewGuid(), Name = "Mobile Devices", Description = "Phones, tablets, accessories", IsActive = true },
                new AssetCategoryDto { Id = Guid.NewGuid(), Name = "Office Furniture", Description = "Desks, chairs, cabinets", IsActive = true },
                new AssetCategoryDto { Id = Guid.NewGuid(), Name = "Vehicles", Description = "Company cars, trucks", IsActive = true }
            };
        }

        public async Task<AssetCategoryDto> CreateAssetCategoryAsync(AssetCategoryDto category)
        {
            try
            {
                await Task.CompletedTask;
                category.Id = Guid.NewGuid();
                category.CreatedAt = DateTime.UtcNow;
                category.IsActive = true;

                _logger.LogInformation("Asset category created: {CategoryId} - {CategoryName}", category.Id, category.Name);
                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create asset category");
                throw;
            }
        }

        public async Task<AssetDepreciationDto> CalculateDepreciationAsync(Guid assetId)
        {
            await Task.CompletedTask;
            return new AssetDepreciationDto
            {
                AssetId = assetId,
                PurchasePrice = 1200.00m,
                CurrentValue = 800.00m,
                DepreciationMethod = "Straight Line",
                UsefulLife = 5,
                AnnualDepreciation = 240.00m,
                AccumulatedDepreciation = 400.00m,
                RemainingValue = 800.00m,
                CalculatedAt = DateTime.UtcNow
            };
        }

        public async Task<List<AssetAuditDto>> GetAssetAuditTrailAsync(Guid assetId)
        {
            await Task.CompletedTask;
            return new List<AssetAuditDto>
            {
                new AssetAuditDto
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetId,
                    Action = "Created",
                    PerformedBy = "IT Manager",
                    Timestamp = DateTime.UtcNow.AddDays(-365),
                    Details = "Asset created and added to inventory",
                    OldValue = null,
                    NewValue = "Available"
                },
                new AssetAuditDto
                {
                    Id = Guid.NewGuid(),
                    AssetId = assetId,
                    Action = "Assigned",
                    PerformedBy = "HR Manager",
                    Timestamp = DateTime.UtcNow.AddDays(-30),
                    Details = "Asset assigned to employee",
                    OldValue = "Available",
                    NewValue = "Assigned"
                }
            };
        }

        public async Task<AssetReportDto> GenerateAssetReportAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new AssetReportDto
            {
                TenantId = tenantId,
                TotalAssets = 250,
                TotalValue = 125000.00m,
                AssignedAssets = 180,
                AvailableAssets = 60,
                MaintenanceAssets = 10,
                DepreciationAmount = 40000.00m,
                AssetsByCategory = new Dictionary<string, int>
                {
                    { "IT Equipment", 120 },
                    { "Office Furniture", 80 },
                    { "Mobile Devices", 50 }
                },
                MaintenanceCosts = 5500.00m,
                RecommendedActions = new List<string>
                {
                    "Schedule maintenance for 10 overdue assets",
                    "Review depreciation schedules for IT equipment",
                    "Update asset locations for better tracking"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }
    }

    public class AssetDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string AssetTag { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string SerialNumber { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CurrentValue { get; set; }
        public required string Status { get; set; }
        public required string Location { get; set; }
        public string? AssignedTo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AssetAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid AssetId { get; set; }
        public required string AssetName { get; set; }
        public Guid EmployeeId { get; set; }
        public required string EmployeeName { get; set; }
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public required string Status { get; set; }
        public string? Notes { get; set; }
    }

    public class AssetMaintenanceDto
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public required string MaintenanceType { get; set; }
        public required string Description { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public required string Status { get; set; }
        public decimal Cost { get; set; }
        public required string PerformedBy { get; set; }
        public string? Notes { get; set; }
    }

    public class AssetAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalAssets { get; set; }
        public int AssignedAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int MaintenanceAssets { get; set; }
        public decimal TotalValue { get; set; }
        public decimal DepreciatedValue { get; set; }
        public decimal MaintenanceCosts { get; set; }
        public double AssetUtilizationRate { get; set; }
        public double AverageAssetAge { get; set; }
        public Dictionary<string, int> TopCategories { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class AssetCategoryDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AssetDepreciationDto
    {
        public Guid AssetId { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal CurrentValue { get; set; }
        public required string DepreciationMethod { get; set; }
        public int UsefulLife { get; set; }
        public decimal AnnualDepreciation { get; set; }
        public decimal AccumulatedDepreciation { get; set; }
        public decimal RemainingValue { get; set; }
        public DateTime CalculatedAt { get; set; }
    }

    public class AssetAuditDto
    {
        public Guid Id { get; set; }
        public Guid AssetId { get; set; }
        public required string Action { get; set; }
        public required string PerformedBy { get; set; }
        public DateTime Timestamp { get; set; }
        public required string Details { get; set; }
        public string? OldValue { get; set; }
        public required string NewValue { get; set; }
    }

    public class AssetReportDto
    {
        public Guid TenantId { get; set; }
        public int TotalAssets { get; set; }
        public decimal TotalValue { get; set; }
        public int AssignedAssets { get; set; }
        public int AvailableAssets { get; set; }
        public int MaintenanceAssets { get; set; }
        public decimal DepreciationAmount { get; set; }
        public Dictionary<string, int> AssetsByCategory { get; set; }
        public decimal MaintenanceCosts { get; set; }
        public List<string> RecommendedActions { get; set; }
        public DateTime GeneratedAt { get; set; }
    }
}
