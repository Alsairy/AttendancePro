using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Workflow.Api.Services;
using Hudur.Shared.Domain.DTOs;

namespace AttendancePlatform.Workflow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AdvancedWorkflowController : ControllerBase
    {
        private readonly IAdvancedWorkflowService _advancedWorkflowService;
        private readonly ILogger<AdvancedWorkflowController> _logger;

        public AdvancedWorkflowController(
            IAdvancedWorkflowService advancedWorkflowService,
            ILogger<AdvancedWorkflowController> logger)
        {
            _advancedWorkflowService = advancedWorkflowService;
            _logger = logger;
        }

        [HttpPost("instances")]
        public async Task<ActionResult<WorkflowInstanceDto>> CreateWorkflowInstance([FromBody] CreateWorkflowInstanceRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var result = await _advancedWorkflowService.CreateWorkflowInstanceAsync(tenantId, request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow instance");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("instances/{workflowInstanceId}/execute")]
        public async Task<ActionResult<WorkflowInstanceDto>> ExecuteWorkflowStep(Guid workflowInstanceId, [FromBody] ExecuteStepRequest request)
        {
            try
            {
                var result = await _advancedWorkflowService.ExecuteWorkflowStepAsync(workflowInstanceId, request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing workflow step for instance {WorkflowInstanceId}", workflowInstanceId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("instances/active")]
        public async Task<ActionResult<List<WorkflowInstanceDto>>> GetActiveWorkflows([FromQuery] string? workflowType = null)
        {
            try
            {
                var tenantId = GetTenantId();
                var result = await _advancedWorkflowService.GetActiveWorkflowsAsync(tenantId, workflowType);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active workflows");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("instances/{workflowInstanceId}")]
        public async Task<ActionResult<WorkflowInstanceDto>> GetWorkflowInstance(Guid workflowInstanceId)
        {
            try
            {
                var result = await _advancedWorkflowService.GetWorkflowInstanceAsync(workflowInstanceId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow instance {WorkflowInstanceId}", workflowInstanceId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("instances/{workflowInstanceId}/cancel")]
        public async Task<ActionResult<bool>> CancelWorkflowInstance(Guid workflowInstanceId, [FromBody] CancelWorkflowRequest request)
        {
            try
            {
                var result = await _advancedWorkflowService.CancelWorkflowInstanceAsync(workflowInstanceId, request.Reason);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling workflow instance {WorkflowInstanceId}", workflowInstanceId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("templates")]
        public async Task<ActionResult<List<WorkflowTemplateDto>>> GetWorkflowTemplates()
        {
            try
            {
                var tenantId = GetTenantId();
                var result = await _advancedWorkflowService.GetWorkflowTemplatesAsync(tenantId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow templates");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("templates")]
        public async Task<ActionResult<WorkflowTemplateDto>> CreateWorkflowTemplate([FromBody] CreateWorkflowTemplateRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var result = await _advancedWorkflowService.CreateWorkflowTemplateAsync(tenantId, request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow template");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("instances/{workflowInstanceId}/logs")]
        public async Task<ActionResult<List<WorkflowExecutionLogDto>>> GetWorkflowExecutionLogs(Guid workflowInstanceId)
        {
            try
            {
                var result = await _advancedWorkflowService.GetWorkflowExecutionLogsAsync(workflowInstanceId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow execution logs for instance {WorkflowInstanceId}", workflowInstanceId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("metrics")]
        public async Task<ActionResult<WorkflowMetricsDto>> GetWorkflowMetrics([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var tenantId = GetTenantId();
                var result = await _advancedWorkflowService.GetWorkflowMetricsAsync(tenantId, startDate, endDate);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow metrics");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpPost("instances/{workflowInstanceId}/steps/{stepId}/retry")]
        public async Task<ActionResult<bool>> RetryFailedWorkflowStep(Guid workflowInstanceId, Guid stepId)
        {
            try
            {
                var result = await _advancedWorkflowService.RetryFailedWorkflowStepAsync(workflowInstanceId, stepId);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrying workflow step {StepId} for instance {WorkflowInstanceId}", stepId, workflowInstanceId);
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("dashboard")]
        public async Task<ActionResult<WorkflowDashboardDto>> GetWorkflowDashboard()
        {
            try
            {
                var tenantId = GetTenantId();
                var currentMonth = DateTime.UtcNow.AddDays(-30);
                var now = DateTime.UtcNow;

                var activeWorkflowsResult = await _advancedWorkflowService.GetActiveWorkflowsAsync(tenantId);
                var metricsResult = await _advancedWorkflowService.GetWorkflowMetricsAsync(tenantId, currentMonth, now);

                if (activeWorkflowsResult.Success && metricsResult.Success)
                {
                    var dashboard = new WorkflowDashboardDto
                    {
                        ActiveWorkflows = activeWorkflowsResult.Data ?? new List<WorkflowInstanceDto>(),
                        Metrics = metricsResult.Data ?? new WorkflowMetricsDto(),
                        PendingApprovals = activeWorkflowsResult.Data?.Where(w => w.CurrentStepName.Contains("Approval")).ToList() ?? new List<WorkflowInstanceDto>(),
                        RecentActivity = activeWorkflowsResult.Data?.OrderByDescending(w => w.UpdatedAt).Take(10).ToList() ?? new List<WorkflowInstanceDto>()
                    };

                    return Ok(ApiResponse<WorkflowDashboardDto>.SuccessResult(dashboard));
                }

                return BadRequest("Failed to load workflow dashboard data");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow dashboard");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        private Guid GetTenantId()
        {
            var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
            if (Guid.TryParse(tenantIdClaim, out var tenantId))
            {
                return tenantId;
            }

            return Guid.Parse("00000000-0000-0000-0000-000000000001");
        }
    }

    public class CancelWorkflowRequest
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class WorkflowDashboardDto
    {
        public List<WorkflowInstanceDto> ActiveWorkflows { get; set; } = new();
        public WorkflowMetricsDto Metrics { get; set; } = new();
        public List<WorkflowInstanceDto> PendingApprovals { get; set; } = new();
        public List<WorkflowInstanceDto> RecentActivity { get; set; } = new();
    }
}
