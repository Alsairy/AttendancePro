using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprehensiveFinanceController : ControllerBase
    {
        private readonly IComprehensiveFinanceService _financeService;
        private readonly ILogger<ComprehensiveFinanceController> _logger;

        public ComprehensiveFinanceController(
            IComprehensiveFinanceService financeService,
            ILogger<ComprehensiveFinanceController> logger)
        {
            _financeService = financeService;
            _logger = logger;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetFinancialDashboard()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var reports = await _financeService.GetFinancialReportsAsync(tenantId);
                var budgetAnalysis = await _financeService.GetBudgetAnalysisAsync(tenantId);
                var cashFlow = await _financeService.GetCashFlowAnalysisAsync(tenantId);

                var dashboard = new
                {
                    TotalRevenue = reports.Sum(r => r.TotalRevenue),
                    TotalExpenses = reports.Sum(r => r.TotalExpenses),
                    NetProfit = reports.Sum(r => r.NetProfit),
                    CashFlow = cashFlow.NetCashFlow,
                    BudgetUtilization = budgetAnalysis.BudgetUtilization
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting financial dashboard");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("reports")]
        public async Task<IActionResult> GetFinancialReports()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var reports = await _financeService.GetFinancialReportsAsync(tenantId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting financial reports");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("budget-analysis")]
        public async Task<IActionResult> GetBudgetAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analysis = await _financeService.GetBudgetAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting budget analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("cash-flow")]
        public async Task<IActionResult> GetCashFlowAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var cashFlow = await _financeService.GetCashFlowAnalysisAsync(tenantId);
                return Ok(cashFlow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cash flow analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("profit-loss")]
        public async Task<IActionResult> GetProfitLossStatement()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var profitLoss = await _financeService.GetProfitLossStatementAsync(tenantId);
                return Ok(profitLoss);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting profit loss statement");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("balance-sheet")]
        public async Task<IActionResult> GetBalanceSheet()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var balanceSheet = await _financeService.GetBalanceSheetAsync(tenantId);
                return Ok(balanceSheet);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting balance sheet");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("expense-reports")]
        public async Task<IActionResult> GetExpenseReports()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var reports = await _financeService.GetExpenseReportsAsync(tenantId);
                return Ok(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting expense reports");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("revenue-analysis")]
        public async Task<IActionResult> GetRevenueAnalysis()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var analysis = await _financeService.GetRevenueAnalysisAsync(tenantId);
                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting revenue analysis");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tax-compliance")]
        public async Task<IActionResult> GetTaxComplianceStatus()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var compliance = await _financeService.GetTaxComplianceStatusAsync(tenantId);
                return Ok(compliance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tax compliance status");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("audit-trail")]
        public async Task<IActionResult> GetFinancialAuditTrail()
        {
            try
            {
                var tenantId = Guid.NewGuid();
                var auditTrail = await _financeService.GetFinancialAuditTrailAsync(tenantId);
                return Ok(auditTrail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting financial audit trail");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
