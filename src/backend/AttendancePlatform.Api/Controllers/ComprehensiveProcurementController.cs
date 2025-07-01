using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveProcurementController : ControllerBase
    {
        private readonly IComprehensiveProcurementService _procurementService;
        private readonly ILogger<ComprehensiveProcurementController> _logger;

        public ComprehensiveProcurementController(
            IComprehensiveProcurementService procurementService,
            ILogger<ComprehensiveProcurementController> logger)
        {
            _procurementService = procurementService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetProcurementDashboard()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var dashboard = await _procurementService.GetProcurementDashboardAsync(tenantId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procurement dashboard");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("purchase-orders")]
        public async Task<IActionResult> GetPurchaseOrders()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var orders = await _procurementService.GetPurchaseOrdersAsync(tenantId);
                return Ok(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting purchase orders");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("vendor-analysis")]
        public async Task<IActionResult> GetVendorAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analysis = await _procurementService.GetVendorAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting vendor analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("contract-management")]
        public async Task<IActionResult> GetContractManagement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var contracts = await _procurementService.GetContractManagementAsync(tenantId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting contract management");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("supplier-performance")]
        public async Task<IActionResult> GetSupplierPerformance()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var performance = await _procurementService.GetSupplierPerformanceAsync(tenantId);
                return Ok(performance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting supplier performance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("spend-analysis")]
        public async Task<IActionResult> GetSpendAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analysis = await _procurementService.GetSpendAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting spend analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("rfq")]
        public async Task<IActionResult> GetRequestForQuotations()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var rfqs = await _procurementService.GetRequestForQuotationsAsync(tenantId);
                return Ok(rfqs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting RFQs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("compliance")]
        public async Task<IActionResult> GetProcurementCompliance()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var compliance = await _procurementService.GetProcurementComplianceAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting procurement compliance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cost-savings")]
        public async Task<IActionResult> GetCostSavingsAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var savings = await _procurementService.GetCostSavingsAnalysisAsync(tenantId);
                return Ok(savings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cost savings analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("purchase-orders")]
        public async Task<IActionResult> CreatePurchaseOrder([FromBody] object purchaseOrder)
        {
            try
            {
                var result = await _procurementService.CreatePurchaseOrderAsync(null);
                return Ok(new { success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
