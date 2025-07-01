using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AttendancePlatform.Api.Services;

namespace AttendancePlatform.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WorkflowEngineController : ControllerBase
    {
        private readonly IWorkflowEngineService _workflowEngineService;
        private readonly ILogger<WorkflowEngineController> _logger;

        public WorkflowEngineController(
            IWorkflowEngineService workflowEngineService,
            ILogger<WorkflowEngineController> logger)
        {
            _workflowEngineService = workflowEngineService;
            _logger = logger;
        }

        [HttpPost("definitions")]
        public async Task<ActionResult<WorkflowDefinitionDto>> CreateWorkflow([FromBody] WorkflowDefinitionDto workflow)
        {
            try
            {
                var createdWorkflow = await _workflowEngineService.CreateWorkflowAsync(workflow);
                return Ok(createdWorkflow);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow definition");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("instances/start")]
        public async Task<ActionResult<WorkflowInstanceDto>> StartWorkflow(
            [FromQuery] Guid workflowId,
            [FromBody] Dictionary<string, object> parameters)
        {
            try
            {
                var instance = await _workflowEngineService.StartWorkflowAsync(workflowId, parameters);
                return Ok(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting workflow instance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("instances/{instanceId}")]
        public async Task<ActionResult<WorkflowInstanceDto>> GetWorkflowInstance(Guid instanceId)
        {
            try
            {
                var instance = await _workflowEngineService.GetWorkflowInstanceAsync(instanceId);
                if (instance == null)
                    return NotFound();

                return Ok(instance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow instance");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("instances/active")]
        public async Task<ActionResult<List<WorkflowInstanceDto>>> GetActiveWorkflows([FromQuery] Guid tenantId)
        {
            try
            {
                var workflows = await _workflowEngineService.GetActiveWorkflowsAsync(tenantId);
                return Ok(workflows);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active workflows");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("tasks/{taskId}/complete")]
        public async Task<ActionResult> CompleteTask(Guid taskId, [FromBody] Dictionary<string, object> outputs)
        {
            try
            {
                var result = await _workflowEngineService.CompleteTaskAsync(taskId, outputs);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing workflow task");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("instances/{instanceId}/cancel")]
        public async Task<ActionResult> CancelWorkflow(Guid instanceId)
        {
            try
            {
                var result = await _workflowEngineService.CancelWorkflowAsync(instanceId);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling workflow");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("tasks/pending")]
        public async Task<ActionResult<List<WorkflowTaskDto>>> GetPendingTasks([FromQuery] Guid userId)
        {
            try
            {
                var tasks = await _workflowEngineService.GetPendingTasksAsync(userId);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending tasks");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("definitions")]
        public async Task<ActionResult<List<WorkflowDefinitionDto>>> GetWorkflowDefinitions([FromQuery] Guid tenantId)
        {
            try
            {
                var definitions = await _workflowEngineService.GetWorkflowDefinitionsAsync(tenantId);
                return Ok(definitions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting workflow definitions");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("instances/{instanceId}/report")]
        public async Task<ActionResult<WorkflowExecutionReportDto>> GetExecutionReport(Guid instanceId)
        {
            try
            {
                var report = await _workflowEngineService.GetExecutionReportAsync(instanceId);
                if (report == null)
                    return NotFound();

                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting execution report");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("definitions/{workflowId}")]
        public async Task<ActionResult> UpdateWorkflowDefinition(Guid workflowId, [FromBody] WorkflowDefinitionDto workflow)
        {
            try
            {
                var result = await _workflowEngineService.UpdateWorkflowDefinitionAsync(workflowId, workflow);
                if (!result)
                    return NotFound();

                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workflow definition");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
