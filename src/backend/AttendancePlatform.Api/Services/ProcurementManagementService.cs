using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Api.Services
{
    public interface IProcurementManagementService
    {
        Task<PurchaseRequestDto> CreatePurchaseRequestAsync(PurchaseRequestDto request);
        Task<List<PurchaseRequestDto>> GetPurchaseRequestsAsync(Guid tenantId);
        Task<PurchaseRequestDto> UpdatePurchaseRequestAsync(Guid requestId, PurchaseRequestDto request);
        Task<bool> ApprovePurchaseRequestAsync(Guid requestId, Guid approverId);
        Task<ProcurementPurchaseOrderDto> CreatePurchaseOrderAsync(ProcurementPurchaseOrderDto order);
        Task<List<ProcurementPurchaseOrderDto>> GetPurchaseOrdersAsync(Guid tenantId);
        Task<ProcurementPurchaseOrderDto> UpdatePurchaseOrderAsync(Guid orderId, ProcurementPurchaseOrderDto order);
        Task<bool> ReceivePurchaseOrderAsync(Guid orderId, ReceiptDto receipt);
        Task<ProcurementSupplierDto> CreateSupplierAsync(ProcurementSupplierDto supplier);
        Task<List<ProcurementSupplierDto>> GetSuppliersAsync(Guid tenantId);
        Task<ProcurementSupplierDto> UpdateSupplierAsync(Guid supplierId, ProcurementSupplierDto supplier);
        Task<SupplierEvaluationDto> CreateSupplierEvaluationAsync(SupplierEvaluationDto evaluation);
        Task<List<SupplierEvaluationDto>> GetSupplierEvaluationsAsync(Guid supplierId);
        Task<ProcurementContractDto> CreateContractAsync(ProcurementContractDto contract);
        Task<List<ProcurementContractDto>> GetContractsAsync(Guid tenantId);
        Task<ProcurementContractDto> UpdateContractAsync(Guid contractId, ProcurementContractDto contract);
        Task<bool> RenewContractAsync(Guid contractId, DateTime newEndDate);
        Task<ProcurementAnalyticsDto> GetProcurementAnalyticsAsync(Guid tenantId);
        Task<ProcurementReportDto> GenerateProcurementReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate);
        Task<List<PurchaseRequestDto>> GetPendingApprovalsAsync(Guid tenantId);
        Task<ProcurementBudgetDto> GetProcurementBudgetAsync(Guid tenantId);
    }

    public class ProcurementManagementService : IProcurementManagementService
    {
        private readonly ILogger<ProcurementManagementService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public ProcurementManagementService(ILogger<ProcurementManagementService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<PurchaseRequestDto> CreatePurchaseRequestAsync(PurchaseRequestDto request)
        {
            try
            {
                request.Id = Guid.NewGuid();
                request.RequestNumber = $"PR-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                request.CreatedAt = DateTime.UtcNow;
                request.Status = "Pending";
                request.Priority = DeterminePriority(request.TotalAmount);

                _logger.LogInformation("Purchase request created: {RequestId} - {RequestNumber}", request.Id, request.RequestNumber);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create purchase request");
                throw;
            }
        }

        public async Task<List<PurchaseRequestDto>> GetPurchaseRequestsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<PurchaseRequestDto>
            {
                new PurchaseRequestDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RequestNumber = "PR-20241227-1001",
                    Title = "Office Supplies Purchase",
                    Description = "Monthly office supplies including stationery, printer paper, and cleaning materials",
                    Category = "Office Supplies",
                    Priority = "Medium",
                    Status = "Pending",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "Office Manager",
                    Department = "Administration",
                    TotalAmount = 1250.00m,
                    Currency = "USD",
                    RequestedDeliveryDate = DateTime.UtcNow.AddDays(7),
                    Justification = "Regular monthly supplies replenishment",
                    ApprovalRequired = true,
                    BudgetCode = "ADMIN-2024-001",
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddHours(-6)
                },
                new PurchaseRequestDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RequestNumber = "PR-20241227-1002",
                    Title = "IT Equipment Upgrade",
                    Description = "New laptops and monitors for development team",
                    Category = "IT Equipment",
                    Priority = "High",
                    Status = "Approved",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "IT Manager",
                    Department = "Information Technology",
                    TotalAmount = 15000.00m,
                    Currency = "USD",
                    RequestedDeliveryDate = DateTime.UtcNow.AddDays(14),
                    Justification = "Performance improvement and team expansion",
                    ApprovalRequired = true,
                    BudgetCode = "IT-2024-003",
                    ApprovedBy = Guid.NewGuid(),
                    ApproverName = "CTO",
                    ApprovalDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        public async Task<PurchaseRequestDto> UpdatePurchaseRequestAsync(Guid requestId, PurchaseRequestDto request)
        {
            try
            {
                await Task.CompletedTask;
                request.Id = requestId;
                request.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Purchase request updated: {RequestId}", requestId);
                return request;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update purchase request {RequestId}", requestId);
                throw;
            }
        }

        public async Task<bool> ApprovePurchaseRequestAsync(Guid requestId, Guid approverId)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Purchase request approved: {RequestId} by {ApproverId}", requestId, approverId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to approve purchase request {RequestId}", requestId);
                return false;
            }
        }

        public async Task<ProcurementPurchaseOrderDto> CreatePurchaseOrderAsync(ProcurementPurchaseOrderDto order)
        {
            try
            {
                order.Id = Guid.NewGuid();
                order.OrderNumber = $"PO-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                order.CreatedAt = DateTime.UtcNow;
                order.Status = "Pending";
                order.OrderDate = DateTime.UtcNow;

                _logger.LogInformation("Purchase order created: {OrderId} - {OrderNumber}", order.Id, order.OrderNumber);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create purchase order");
                throw;
            }
        }

        public async Task<List<ProcurementPurchaseOrderDto>> GetPurchaseOrdersAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<ProcurementPurchaseOrderDto>
            {
                new ProcurementPurchaseOrderDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    OrderNumber = "PO-20241227-1001",
                    PurchaseRequestId = Guid.NewGuid(),
                    SupplierId = Guid.NewGuid(),
                    SupplierName = "Office Supplies Co.",
                    SupplierContact = "sales@officesupplies.com",
                    OrderDate = DateTime.UtcNow.AddDays(-1),
                    ExpectedDeliveryDate = DateTime.UtcNow.AddDays(5),
                    Status = "Sent",
                    TotalAmount = 1250.00m,
                    Currency = "USD",
                    PaymentTerms = "Net 30",
                    DeliveryAddress = "123 Business Ave, Corporate City, CC 12345",
                    SpecialInstructions = "Deliver to main reception desk",
                    CreatedBy = Guid.NewGuid(),
                    CreatorName = "Procurement Manager",
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    UpdatedAt = DateTime.UtcNow.AddHours(-2)
                },
                new ProcurementPurchaseOrderDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    OrderNumber = "PO-20241227-1002",
                    PurchaseRequestId = Guid.NewGuid(),
                    SupplierId = Guid.NewGuid(),
                    SupplierName = "TechGear Solutions",
                    SupplierContact = "orders@techgear.com",
                    OrderDate = DateTime.UtcNow.AddDays(-3),
                    ExpectedDeliveryDate = DateTime.UtcNow.AddDays(10),
                    Status = "Received",
                    TotalAmount = 15000.00m,
                    Currency = "USD",
                    PaymentTerms = "Net 15",
                    DeliveryAddress = "123 Business Ave, Corporate City, CC 12345",
                    SpecialInstructions = "Coordinate with IT department for setup",
                    CreatedBy = Guid.NewGuid(),
                    CreatorName = "Procurement Manager",
                    ReceivedDate = DateTime.UtcNow.AddDays(-1),
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    UpdatedAt = DateTime.UtcNow.AddDays(-1)
                }
            };
        }

        public async Task<ProcurementPurchaseOrderDto> UpdatePurchaseOrderAsync(Guid orderId, ProcurementPurchaseOrderDto order)
        {
            try
            {
                await Task.CompletedTask;
                order.Id = orderId;
                order.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Purchase order updated: {OrderId}", orderId);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update purchase order {OrderId}", orderId);
                throw;
            }
        }

        public async Task<bool> ReceivePurchaseOrderAsync(Guid orderId, ReceiptDto receipt)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Purchase order received: {OrderId}", orderId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to receive purchase order {OrderId}", orderId);
                return false;
            }
        }

        public async Task<ProcurementSupplierDto> CreateSupplierAsync(ProcurementSupplierDto supplier)
        {
            try
            {
                supplier.Id = Guid.NewGuid();
                supplier.SupplierCode = $"SUP-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                supplier.CreatedAt = DateTime.UtcNow;
                supplier.Status = "Active";
                supplier.Rating = 0.0;

                _logger.LogInformation("Supplier created: {SupplierId} - {SupplierCode}", supplier.Id, supplier.SupplierCode);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create supplier");
                throw;
            }
        }

        public async Task<List<ProcurementSupplierDto>> GetSuppliersAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<ProcurementSupplierDto>
            {
                new ProcurementSupplierDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    SupplierCode = "SUP-20241227-1001",
                    CompanyName = "Office Supplies Co.",
                    ContactName = "John Supplier",
                    ContactEmail = "john@officesupplies.com",
                    ContactPhone = "+1234567890",
                    Address = "456 Supply Street, Vendor City, VC 67890",
                    TaxId = "TAX123456789",
                    PaymentTerms = "Net 30",
                    Categories = new List<string> { "Office Supplies", "Stationery", "Cleaning Materials" },
                    Rating = 4.5,
                    Status = "Active",
                    CertificationLevel = "ISO 9001",
                    DeliveryCapability = "Regional",
                    MinimumOrderValue = 100.00m,
                    Currency = "USD",
                    CreatedAt = DateTime.UtcNow.AddDays(-180),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new ProcurementSupplierDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    SupplierCode = "SUP-20241227-1002",
                    CompanyName = "TechGear Solutions",
                    ContactName = "Sarah Technology",
                    ContactEmail = "sarah@techgear.com",
                    ContactPhone = "+1234567891",
                    Address = "789 Tech Boulevard, Innovation City, IC 13579",
                    TaxId = "TAX987654321",
                    PaymentTerms = "Net 15",
                    Categories = new List<string> { "IT Equipment", "Software", "Hardware" },
                    Rating = 4.8,
                    Status = "Active",
                    CertificationLevel = "ISO 27001",
                    DeliveryCapability = "National",
                    MinimumOrderValue = 500.00m,
                    Currency = "USD",
                    CreatedAt = DateTime.UtcNow.AddDays(-365),
                    UpdatedAt = DateTime.UtcNow.AddDays(-15)
                }
            };
        }

        public async Task<ProcurementSupplierDto> UpdateSupplierAsync(Guid supplierId, ProcurementSupplierDto supplier)
        {
            try
            {
                await Task.CompletedTask;
                supplier.Id = supplierId;
                supplier.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Supplier updated: {SupplierId}", supplierId);
                return supplier;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update supplier {SupplierId}", supplierId);
                throw;
            }
        }

        public async Task<SupplierEvaluationDto> CreateSupplierEvaluationAsync(SupplierEvaluationDto evaluation)
        {
            try
            {
                evaluation.Id = Guid.NewGuid();
                evaluation.EvaluationNumber = $"SE-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                evaluation.CreatedAt = DateTime.UtcNow;
                evaluation.EvaluationDate = DateTime.UtcNow;

                _logger.LogInformation("Supplier evaluation created: {EvaluationId} - {EvaluationNumber}", evaluation.Id, evaluation.EvaluationNumber);
                return evaluation;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create supplier evaluation");
                throw;
            }
        }

        public async Task<List<SupplierEvaluationDto>> GetSupplierEvaluationsAsync(Guid supplierId)
        {
            await Task.CompletedTask;
            return new List<SupplierEvaluationDto>
            {
                new SupplierEvaluationDto
                {
                    Id = Guid.NewGuid(),
                    SupplierId = supplierId,
                    EvaluationNumber = "SE-20241227-1001",
                    EvaluationDate = DateTime.UtcNow.AddDays(-30),
                    EvaluatedBy = Guid.NewGuid(),
                    EvaluatorName = "Procurement Manager",
                    Period = "Q4 2024",
                    QualityScore = 4.5,
                    DeliveryScore = 4.2,
                    ServiceScore = 4.7,
                    PricingScore = 4.0,
                    OverallScore = 4.35,
                    Comments = "Excellent quality and service. Delivery times could be improved.",
                    Recommendations = "Consider expedited shipping options for urgent orders",
                    NextEvaluationDate = DateTime.UtcNow.AddDays(60),
                    CreatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new SupplierEvaluationDto
                {
                    Id = Guid.NewGuid(),
                    SupplierId = supplierId,
                    EvaluationNumber = "SE-20241227-1002",
                    EvaluationDate = DateTime.UtcNow.AddDays(-120),
                    EvaluatedBy = Guid.NewGuid(),
                    EvaluatorName = "Procurement Manager",
                    Period = "Q3 2024",
                    QualityScore = 4.3,
                    DeliveryScore = 4.0,
                    ServiceScore = 4.5,
                    PricingScore = 4.2,
                    OverallScore = 4.25,
                    Comments = "Good overall performance with room for improvement in delivery",
                    Recommendations = "Implement better tracking system for shipments",
                    NextEvaluationDate = DateTime.UtcNow.AddDays(-30),
                    CreatedAt = DateTime.UtcNow.AddDays(-120)
                }
            };
        }

        public async Task<ProcurementContractDto> CreateContractAsync(ProcurementContractDto contract)
        {
            try
            {
                contract.Id = Guid.NewGuid();
                contract.ContractNumber = $"CT-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                contract.CreatedAt = DateTime.UtcNow;
                contract.Status = "Draft";

                _logger.LogInformation("Contract created: {ContractId} - {ContractNumber}", contract.Id, contract.ContractNumber);
                return contract;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create contract");
                throw;
            }
        }

        public async Task<List<ProcurementContractDto>> GetContractsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<ProcurementContractDto>
            {
                new ProcurementContractDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ContractNumber = "CT-20241227-1001",
                    SupplierId = Guid.NewGuid(),
                    SupplierName = "Office Supplies Co.",
                    ContractType = "Supply Agreement",
                    Title = "Annual Office Supplies Contract",
                    Description = "Comprehensive supply agreement for office materials and stationery",
                    StartDate = DateTime.UtcNow.AddDays(-90),
                    EndDate = DateTime.UtcNow.AddDays(275),
                    RenewalDate = DateTime.UtcNow.AddDays(245),
                    ContractValue = 50000.00m,
                    Currency = "USD",
                    PaymentTerms = "Net 30",
                    Status = "Active",
                    AutoRenewal = true,
                    RenewalTerms = "12 months with 5% price adjustment",
                    CreatedBy = Guid.NewGuid(),
                    CreatorName = "Procurement Manager",
                    CreatedAt = DateTime.UtcNow.AddDays(-90),
                    UpdatedAt = DateTime.UtcNow.AddDays(-30)
                },
                new ProcurementContractDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    ContractNumber = "CT-20241227-1002",
                    SupplierId = Guid.NewGuid(),
                    SupplierName = "TechGear Solutions",
                    ContractType = "Service Agreement",
                    Title = "IT Equipment Maintenance Contract",
                    Description = "Annual maintenance and support contract for IT equipment",
                    StartDate = DateTime.UtcNow.AddDays(-180),
                    EndDate = DateTime.UtcNow.AddDays(185),
                    RenewalDate = DateTime.UtcNow.AddDays(155),
                    ContractValue = 25000.00m,
                    Currency = "USD",
                    PaymentTerms = "Net 15",
                    Status = "Active",
                    AutoRenewal = false,
                    RenewalTerms = "Manual renewal required",
                    CreatedBy = Guid.NewGuid(),
                    CreatorName = "IT Manager",
                    CreatedAt = DateTime.UtcNow.AddDays(-180),
                    UpdatedAt = DateTime.UtcNow.AddDays(-60)
                }
            };
        }

        public async Task<ProcurementContractDto> UpdateContractAsync(Guid contractId, ProcurementContractDto contract)
        {
            try
            {
                await Task.CompletedTask;
                contract.Id = contractId;
                contract.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Contract updated: {ContractId}", contractId);
                return contract;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update contract {ContractId}", contractId);
                throw;
            }
        }

        public async Task<bool> RenewContractAsync(Guid contractId, DateTime newEndDate)
        {
            try
            {
                await Task.CompletedTask;
                _logger.LogInformation("Contract renewed: {ContractId} until {EndDate}", contractId, newEndDate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to renew contract {ContractId}", contractId);
                return false;
            }
        }

        public async Task<ProcurementAnalyticsDto> GetProcurementAnalyticsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new ProcurementAnalyticsDto
            {
                TenantId = tenantId,
                TotalPurchaseRequests = 285,
                PendingRequests = 45,
                ApprovedRequests = 185,
                RejectedRequests = 55,
                TotalPurchaseOrders = 165,
                TotalSpend = 850000.00m,
                AverageOrderValue = 5151.52m,
                TopCategories = new Dictionary<string, decimal>
                {
                    { "IT Equipment", 325000.00m },
                    { "Office Supplies", 185000.00m },
                    { "Maintenance", 125000.00m },
                    { "Professional Services", 95000.00m },
                    { "Marketing", 65000.00m },
                    { "Travel", 50000.00m }
                },
                SupplierPerformance = new Dictionary<string, double>
                {
                    { "TechGear Solutions", 4.8 },
                    { "Office Supplies Co.", 4.5 },
                    { "Professional Services Inc.", 4.3 },
                    { "Maintenance Pro", 4.1 }
                },
                MonthlySpendTrends = new Dictionary<string, decimal>
                {
                    { "Jan", 65000.00m }, { "Feb", 58000.00m }, { "Mar", 72000.00m }, { "Apr", 68000.00m },
                    { "May", 85000.00m }, { "Jun", 78000.00m }, { "Jul", 62000.00m }, { "Aug", 75000.00m },
                    { "Sep", 92000.00m }, { "Oct", 88000.00m }, { "Nov", 70000.00m }, { "Dec", 95000.00m }
                },
                CostSavings = new Dictionary<string, decimal>
                {
                    { "Bulk Purchasing", 25000.00m },
                    { "Negotiated Discounts", 18000.00m },
                    { "Contract Optimization", 12000.00m },
                    { "Supplier Consolidation", 8000.00m }
                },
                ProcessEfficiency = new Dictionary<string, double>
                {
                    { "Average Approval Time (days)", 3.2 },
                    { "Order Processing Time (days)", 1.8 },
                    { "Supplier Response Time (days)", 2.1 },
                    { "Delivery Accuracy (%)", 94.5 }
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<ProcurementReportDto> GenerateProcurementReportAsync(Guid tenantId, DateTime fromDate, DateTime toDate)
        {
            await Task.CompletedTask;
            return new ProcurementReportDto
            {
                TenantId = tenantId,
                ReportPeriod = $"{fromDate:yyyy-MM-dd} to {toDate:yyyy-MM-dd}",
                TotalRequests = 125,
                ApprovedRequests = 98,
                PendingRequests = 18,
                RejectedRequests = 9,
                TotalOrders = 85,
                TotalSpend = 425000.00m,
                AverageOrderValue = 5000.00m,
                TopSuppliers = new List<string>
                {
                    "TechGear Solutions - $185,000",
                    "Office Supplies Co. - $95,000",
                    "Professional Services Inc. - $75,000",
                    "Maintenance Pro - $45,000",
                    "Marketing Solutions - $25,000"
                },
                CategoryBreakdown = new Dictionary<string, ProcurementCategoryStatsDto>
                {
                    { "IT Equipment", new ProcurementCategoryStatsDto { Category = "IT Equipment", Orders = 25, Spend = 185000.00m, Savings = 8500.00m } },
                    { "Office Supplies", new ProcurementCategoryStatsDto { Category = "Office Supplies", Orders = 35, Spend = 95000.00m, Savings = 4200.00m } },
                    { "Professional Services", new ProcurementCategoryStatsDto { Category = "Professional Services", Orders = 15, Spend = 75000.00m, Savings = 3800.00m } },
                    { "Maintenance", new ProcurementCategoryStatsDto { Category = "Maintenance", Orders = 10, Spend = 45000.00m, Savings = 2100.00m } }
                },
                ComplianceMetrics = new Dictionary<string, double>
                {
                    { "Policy Compliance (%)", 96.5 },
                    { "Contract Compliance (%)", 94.2 },
                    { "Budget Compliance (%)", 98.1 },
                    { "Approval Workflow Compliance (%)", 99.2 }
                },
                PerformanceMetrics = new Dictionary<string, double>
                {
                    { "On-Time Delivery (%)", 92.8 },
                    { "Quality Acceptance (%)", 95.5 },
                    { "Supplier Satisfaction", 4.3 },
                    { "Cost Variance (%)", -2.1 }
                },
                CostSavingsAchieved = 18600.00m,
                BudgetUtilization = 85.2,
                Recommendations = new List<string>
                {
                    "Consolidate suppliers in office supplies category for better pricing",
                    "Implement automated approval workflows for low-value purchases",
                    "Negotiate volume discounts with top-performing suppliers",
                    "Consider long-term contracts for recurring purchases"
                },
                GeneratedAt = DateTime.UtcNow
            };
        }

        public async Task<List<PurchaseRequestDto>> GetPendingApprovalsAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new List<PurchaseRequestDto>
            {
                new PurchaseRequestDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RequestNumber = "PR-20241227-1003",
                    Title = "Marketing Campaign Materials",
                    Description = "Promotional materials for Q1 marketing campaign",
                    Category = "Marketing",
                    Priority = "High",
                    Status = "Pending",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "Marketing Manager",
                    Department = "Marketing",
                    TotalAmount = 8500.00m,
                    Currency = "USD",
                    RequestedDeliveryDate = DateTime.UtcNow.AddDays(10),
                    Justification = "Time-sensitive campaign launch materials",
                    ApprovalRequired = true,
                    BudgetCode = "MKT-2024-Q1",
                    DaysWaiting = 2,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    UpdatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new PurchaseRequestDto
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    RequestNumber = "PR-20241227-1004",
                    Title = "Training Room Equipment",
                    Description = "Projector and audio equipment for new training facility",
                    Category = "Training Equipment",
                    Priority = "Medium",
                    Status = "Pending",
                    RequestedBy = Guid.NewGuid(),
                    RequesterName = "Training Coordinator",
                    Department = "Human Resources",
                    TotalAmount = 3200.00m,
                    Currency = "USD",
                    RequestedDeliveryDate = DateTime.UtcNow.AddDays(21),
                    Justification = "Equipment needed for new employee training programs",
                    ApprovalRequired = true,
                    BudgetCode = "HR-2024-002",
                    DaysWaiting = 5,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    UpdatedAt = DateTime.UtcNow.AddDays(-3)
                }
            };
        }

        public async Task<ProcurementBudgetDto> GetProcurementBudgetAsync(Guid tenantId)
        {
            await Task.CompletedTask;
            return new ProcurementBudgetDto
            {
                TenantId = tenantId,
                FiscalYear = DateTime.UtcNow.Year,
                TotalBudget = 1000000.00m,
                SpentToDate = 650000.00m,
                CommittedAmount = 125000.00m,
                AvailableBudget = 225000.00m,
                BudgetUtilization = 65.0,
                CategoryBudgets = new Dictionary<string, ProcurementBudgetCategoryDto>
                {
                    { "IT Equipment", new ProcurementBudgetCategoryDto { Category = "IT Equipment", Status = "Active", Allocated = 400000.00m, Spent = 285000.00m, Committed = 50000.00m, Available = 65000.00m } },
                    { "Office Supplies", new ProcurementBudgetCategoryDto { Category = "Office Supplies", Status = "Active", Allocated = 200000.00m, Spent = 145000.00m, Committed = 25000.00m, Available = 30000.00m } },
                    { "Professional Services", new ProcurementBudgetCategoryDto { Category = "Professional Services", Status = "Active", Allocated = 150000.00m, Spent = 95000.00m, Committed = 20000.00m, Available = 35000.00m } },
                    { "Maintenance", new ProcurementBudgetCategoryDto { Category = "Maintenance", Status = "Active", Allocated = 100000.00m, Spent = 65000.00m, Committed = 15000.00m, Available = 20000.00m } },
                    { "Marketing", new ProcurementBudgetCategoryDto { Category = "Marketing", Status = "Active", Allocated = 100000.00m, Spent = 45000.00m, Committed = 10000.00m, Available = 45000.00m } },
                    { "Travel", new ProcurementBudgetCategoryDto { Category = "Travel", Status = "Active", Allocated = 50000.00m, Spent = 15000.00m, Committed = 5000.00m, Available = 30000.00m } }
                },
                QuarterlySpending = new Dictionary<string, decimal>
                {
                    { "Q1", 185000.00m },
                    { "Q2", 225000.00m },
                    { "Q3", 165000.00m },
                    { "Q4", 75000.00m }
                },
                ProjectedYearEndSpend = 875000.00m,
                BudgetVariance = -125000.00m,
                SavingsTarget = 50000.00m,
                SavingsAchieved = 32500.00m,
                GeneratedAt = DateTime.UtcNow
            };
        }

        private string DeterminePriority(decimal amount)
        {
            if (amount >= 10000) return "High";
            if (amount >= 5000) return "Medium";
            return "Low";
        }
    }

    public class PurchaseRequestDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string RequestNumber { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Category { get; set; }
        public required string Priority { get; set; }
        public required string Status { get; set; }
        public Guid RequestedBy { get; set; }
        public required string RequesterName { get; set; }
        public required string Department { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Currency { get; set; }
        public DateTime RequestedDeliveryDate { get; set; }
        public required string Justification { get; set; }
        public bool ApprovalRequired { get; set; }
        public required string BudgetCode { get; set; }
        public Guid? ApprovedBy { get; set; }
        public string? ApproverName { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public int DaysWaiting { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProcurementPurchaseOrderDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string OrderNumber { get; set; }
        public Guid PurchaseRequestId { get; set; }
        public Guid SupplierId { get; set; }
        public required string SupplierName { get; set; }
        public required string SupplierContact { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public required string Status { get; set; }
        public decimal TotalAmount { get; set; }
        public required string Currency { get; set; }
        public required string PaymentTerms { get; set; }
        public required string DeliveryAddress { get; set; }
        public required string SpecialInstructions { get; set; }
        public Guid CreatedBy { get; set; }
        public required string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReceiptDto
    {
        public DateTime ReceivedDate { get; set; }
        public Guid ReceivedBy { get; set; }
        public required string ReceiverName { get; set; }
        public required string ReceiptNotes { get; set; }
        public bool FullyReceived { get; set; }
        public List<string> PartialItems { get; set; }
        public required string QualityStatus { get; set; }
    }

    public class ProcurementSupplierDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string SupplierCode { get; set; }
        public required string CompanyName { get; set; }
        public required string ContactName { get; set; }
        public required string ContactEmail { get; set; }
        public required string ContactPhone { get; set; }
        public required string Address { get; set; }
        public required string TaxId { get; set; }
        public required string PaymentTerms { get; set; }
        public List<string> Categories { get; set; }
        public double Rating { get; set; }
        public required string Status { get; set; }
        public required string CertificationLevel { get; set; }
        public required string DeliveryCapability { get; set; }
        public decimal MinimumOrderValue { get; set; }
        public required string Currency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class SupplierEvaluationDto
    {
        public Guid Id { get; set; }
        public Guid SupplierId { get; set; }
        public required string EvaluationNumber { get; set; }
        public DateTime EvaluationDate { get; set; }
        public Guid EvaluatedBy { get; set; }
        public required string EvaluatorName { get; set; }
        public required string Period { get; set; }
        public double QualityScore { get; set; }
        public double DeliveryScore { get; set; }
        public double ServiceScore { get; set; }
        public double PricingScore { get; set; }
        public double OverallScore { get; set; }
        public required string Comments { get; set; }
        public required string Recommendations { get; set; }
        public DateTime NextEvaluationDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProcurementContractDto
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public required string ContractNumber { get; set; }
        public Guid SupplierId { get; set; }
        public required string SupplierName { get; set; }
        public required string ContractType { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RenewalDate { get; set; }
        public decimal ContractValue { get; set; }
        public required string Currency { get; set; }
        public required string PaymentTerms { get; set; }
        public required string Status { get; set; }
        public bool AutoRenewal { get; set; }
        public required string RenewalTerms { get; set; }
        public Guid CreatedBy { get; set; }
        public required string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ProcurementAnalyticsDto
    {
        public Guid TenantId { get; set; }
        public int TotalPurchaseRequests { get; set; }
        public int PendingRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalPurchaseOrders { get; set; }
        public decimal TotalSpend { get; set; }
        public decimal AverageOrderValue { get; set; }
        public Dictionary<string, decimal> TopCategories { get; set; }
        public Dictionary<string, double> SupplierPerformance { get; set; }
        public Dictionary<string, decimal> MonthlySpendTrends { get; set; }
        public Dictionary<string, decimal> CostSavings { get; set; }
        public Dictionary<string, double> ProcessEfficiency { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class ProcurementReportDto
    {
        public Guid TenantId { get; set; }
        public required string ReportPeriod { get; set; }
        public int TotalRequests { get; set; }
        public int ApprovedRequests { get; set; }
        public int PendingRequests { get; set; }
        public int RejectedRequests { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpend { get; set; }
        public decimal AverageOrderValue { get; set; }
        public List<string> TopSuppliers { get; set; }
        public Dictionary<string, ProcurementCategoryStatsDto> CategoryBreakdown { get; set; }
        public Dictionary<string, double> ComplianceMetrics { get; set; }
        public Dictionary<string, double> PerformanceMetrics { get; set; }
        public decimal CostSavingsAchieved { get; set; }
        public double BudgetUtilization { get; set; }
        public List<string> Recommendations { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class ProcurementCategoryStatsDto
    {
        public required string Category { get; set; }
        public int Orders { get; set; }
        public decimal Spend { get; set; }
        public decimal Savings { get; set; }
    }

    public class ProcurementBudgetDto
    {
        public Guid TenantId { get; set; }
        public int FiscalYear { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal SpentToDate { get; set; }
        public decimal CommittedAmount { get; set; }
        public decimal AvailableBudget { get; set; }
        public double BudgetUtilization { get; set; }
        public Dictionary<string, ProcurementBudgetCategoryDto> CategoryBudgets { get; set; }
        public Dictionary<string, decimal> QuarterlySpending { get; set; }
        public decimal ProjectedYearEndSpend { get; set; }
        public decimal BudgetVariance { get; set; }
        public decimal SavingsTarget { get; set; }
        public decimal SavingsAchieved { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class ProcurementBudgetCategoryDto
    {
        public required string Category { get; set; }
        public required string Status { get; set; }
        public decimal Allocated { get; set; }
        public decimal Spent { get; set; }
        public decimal Committed { get; set; }
        public decimal Available { get; set; }
    }
}
