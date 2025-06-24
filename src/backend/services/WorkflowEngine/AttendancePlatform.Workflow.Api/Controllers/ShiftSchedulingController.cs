using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AttendancePlatform.Workflow.Api.Services;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Workflow.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShiftSchedulingController : ControllerBase
    {
        private readonly IShiftSchedulingService _shiftSchedulingService;
        private readonly ILogger<ShiftSchedulingController> _logger;

        public ShiftSchedulingController(
            IShiftSchedulingService shiftSchedulingService,
            ILogger<ShiftSchedulingController> logger)
        {
            _shiftSchedulingService = shiftSchedulingService;
            _logger = logger;
        }

        [HttpPost("templates")]
        public async Task<ActionResult<ShiftTemplate>> CreateShiftTemplate([FromBody] CreateShiftTemplateRequest request)
        {
            try
            {
                var template = await _shiftSchedulingService.CreateShiftTemplateAsync(request);
                return CreatedAtAction(nameof(GetShiftTemplate), new { id = template.Id }, template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shift template");
                return BadRequest(new { message = "Failed to create shift template", error = ex.Message });
            }
        }

        [HttpGet("templates")]
        public async Task<ActionResult<List<ShiftTemplate>>> GetShiftTemplates([FromQuery] string tenantId)
        {
            try
            {
                var templates = await _shiftSchedulingService.GetShiftTemplatesAsync(tenantId);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift templates for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get shift templates", error = ex.Message });
            }
        }

        [HttpGet("templates/{id}")]
        public async Task<ActionResult<ShiftTemplate>> GetShiftTemplate(string id)
        {
            try
            {
                var template = await _shiftSchedulingService.GetShiftTemplateAsync(id);
                if (template == null)
                {
                    return NotFound();
                }
                return Ok(template);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift template {TemplateId}", id);
                return BadRequest(new { message = "Failed to get shift template", error = ex.Message });
            }
        }

        [HttpPut("templates/{id}")]
        public async Task<ActionResult<ShiftTemplate>> UpdateShiftTemplate(string id, [FromBody] UpdateShiftTemplateRequest request)
        {
            try
            {
                var template = await _shiftSchedulingService.UpdateShiftTemplateAsync(id, request);
                return Ok(template);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shift template {TemplateId}", id);
                return BadRequest(new { message = "Failed to update shift template", error = ex.Message });
            }
        }

        [HttpDelete("templates/{id}")]
        public async Task<ActionResult> DeleteShiftTemplate(string id)
        {
            try
            {
                var result = await _shiftSchedulingService.DeleteShiftTemplateAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shift template {TemplateId}", id);
                return BadRequest(new { message = "Failed to delete shift template", error = ex.Message });
            }
        }

        [HttpPost("shifts")]
        public async Task<ActionResult<Shift>> CreateShift([FromBody] CreateShiftRequest request)
        {
            try
            {
                var shift = await _shiftSchedulingService.CreateShiftAsync(request);
                return CreatedAtAction(nameof(GetShift), new { id = shift.Id }, shift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating shift");
                return BadRequest(new { message = "Failed to create shift", error = ex.Message });
            }
        }

        [HttpGet("shifts")]
        public async Task<ActionResult<List<Shift>>> GetShifts(
            [FromQuery] string tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var shifts = await _shiftSchedulingService.GetShiftsAsync(tenantId, startDate, endDate);
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shifts for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get shifts", error = ex.Message });
            }
        }

        [HttpGet("shifts/{id}")]
        public async Task<ActionResult<Shift>> GetShift(string id)
        {
            try
            {
                var shift = await _shiftSchedulingService.GetShiftAsync(id);
                if (shift == null)
                {
                    return NotFound();
                }
                return Ok(shift);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift {ShiftId}", id);
                return BadRequest(new { message = "Failed to get shift", error = ex.Message });
            }
        }

        [HttpPut("shifts/{id}")]
        public async Task<ActionResult<Shift>> UpdateShift(string id, [FromBody] UpdateShiftRequest request)
        {
            try
            {
                var shift = await _shiftSchedulingService.UpdateShiftAsync(id, request);
                return Ok(shift);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating shift {ShiftId}", id);
                return BadRequest(new { message = "Failed to update shift", error = ex.Message });
            }
        }

        [HttpDelete("shifts/{id}")]
        public async Task<ActionResult> DeleteShift(string id)
        {
            try
            {
                var result = await _shiftSchedulingService.DeleteShiftAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting shift {ShiftId}", id);
                return BadRequest(new { message = "Failed to delete shift", error = ex.Message });
            }
        }

        [HttpPost("assignments")]
        public async Task<ActionResult<ShiftAssignment>> AssignShift([FromBody] AssignShiftRequest request)
        {
            try
            {
                var assignment = await _shiftSchedulingService.AssignShiftAsync(request);
                return CreatedAtAction(nameof(GetShiftAssignment), new { id = assignment.Id }, assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning shift");
                return BadRequest(new { message = "Failed to assign shift", error = ex.Message });
            }
        }

        [HttpGet("assignments")]
        public async Task<ActionResult<List<ShiftAssignment>>> GetShiftAssignments(
            [FromQuery] string tenantId,
            [FromQuery] string? userId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var assignments = await _shiftSchedulingService.GetShiftAssignmentsAsync(tenantId, userId, startDate, endDate);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift assignments for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get shift assignments", error = ex.Message });
            }
        }

        [HttpGet("assignments/{id}")]
        public async Task<ActionResult<ShiftAssignment>> GetShiftAssignment(string id)
        {
            try
            {
                var assignment = await _shiftSchedulingService.GetShiftAssignmentAsync(id);
                if (assignment == null)
                {
                    return NotFound();
                }
                return Ok(assignment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift assignment {AssignmentId}", id);
                return BadRequest(new { message = "Failed to get shift assignment", error = ex.Message });
            }
        }

        [HttpDelete("assignments/{id}")]
        public async Task<ActionResult> UnassignShift(string id)
        {
            try
            {
                var result = await _shiftSchedulingService.UnassignShiftAsync(id);
                if (!result)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unassigning shift {AssignmentId}", id);
                return BadRequest(new { message = "Failed to unassign shift", error = ex.Message });
            }
        }

        [HttpPost("swap-requests")]
        public async Task<ActionResult<ShiftSwapRequest>> CreateSwapRequest([FromBody] CreateSwapRequestRequest request)
        {
            try
            {
                var swapRequest = await _shiftSchedulingService.CreateSwapRequestAsync(request);
                return CreatedAtAction(nameof(GetSwapRequest), new { id = swapRequest.Id }, swapRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating swap request");
                return BadRequest(new { message = "Failed to create swap request", error = ex.Message });
            }
        }

        [HttpGet("swap-requests")]
        public async Task<ActionResult<List<ShiftSwapRequest>>> GetSwapRequests(
            [FromQuery] string tenantId,
            [FromQuery] string? userId = null,
            [FromQuery] SwapRequestStatus? status = null)
        {
            try
            {
                var swapRequests = await _shiftSchedulingService.GetSwapRequestsAsync(tenantId, userId, status);
                return Ok(swapRequests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting swap requests for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get swap requests", error = ex.Message });
            }
        }

        [HttpGet("swap-requests/{id}")]
        public async Task<ActionResult<ShiftSwapRequest>> GetSwapRequest(string id)
        {
            try
            {
                var swapRequest = await _shiftSchedulingService.GetSwapRequestAsync(id);
                if (swapRequest == null)
                {
                    return NotFound();
                }
                return Ok(swapRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting swap request {RequestId}", id);
                return BadRequest(new { message = "Failed to get swap request", error = ex.Message });
            }
        }

        [HttpPut("swap-requests/{id}/process")]
        public async Task<ActionResult<ShiftSwapRequest>> ProcessSwapRequest(string id, [FromBody] ProcessSwapRequestRequest request)
        {
            try
            {
                var swapRequest = await _shiftSchedulingService.ProcessSwapRequestAsync(id, request);
                return Ok(swapRequest);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing swap request {RequestId}", id);
                return BadRequest(new { message = "Failed to process swap request", error = ex.Message });
            }
        }

        [HttpGet("conflicts")]
        public async Task<ActionResult<List<ShiftConflict>>> GetConflicts(
            [FromQuery] string tenantId,
            [FromQuery] ConflictStatus? status = null)
        {
            try
            {
                var conflicts = await _shiftSchedulingService.GetConflictsAsync(tenantId, status);
                return Ok(conflicts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conflicts for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get conflicts", error = ex.Message });
            }
        }

        [HttpPost("conflicts/detect")]
        public async Task<ActionResult<List<ShiftConflict>>> DetectConflicts(
            [FromQuery] string tenantId,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var conflicts = await _shiftSchedulingService.DetectConflictsAsync(tenantId, startDate, endDate);
                return Ok(conflicts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error detecting conflicts for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to detect conflicts", error = ex.Message });
            }
        }

        [HttpPut("conflicts/{id}/resolve")]
        public async Task<ActionResult<ShiftConflict>> ResolveConflict(string id, [FromBody] ResolveConflictRequest request)
        {
            try
            {
                var conflict = await _shiftSchedulingService.ResolveConflictAsync(id, request);
                return Ok(conflict);
            }
            catch (ArgumentException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving conflict {ConflictId}", id);
                return BadRequest(new { message = "Failed to resolve conflict", error = ex.Message });
            }
        }

        [HttpPost("schedule/generate")]
        public async Task<ActionResult<ShiftScheduleDto>> GenerateSchedule([FromBody] GenerateScheduleRequest request)
        {
            try
            {
                var schedule = await _shiftSchedulingService.GenerateScheduleAsync(request);
                return Ok(schedule);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating schedule for tenant {TenantId}", request.TenantId);
                return BadRequest(new { message = "Failed to generate schedule", error = ex.Message });
            }
        }

        [HttpPost("assignments/auto-assign")]
        public async Task<ActionResult<List<ShiftAssignment>>> AutoAssignShifts([FromBody] AutoAssignRequest request)
        {
            try
            {
                var assignments = await _shiftSchedulingService.AutoAssignShiftsAsync(request);
                return Ok(assignments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error auto-assigning shifts for tenant {TenantId}", request.TenantId);
                return BadRequest(new { message = "Failed to auto-assign shifts", error = ex.Message });
            }
        }

        [HttpGet("coverage/report")]
        public async Task<ActionResult<ShiftCoverageReport>> GetCoverageReport(
            [FromQuery] string tenantId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var report = await _shiftSchedulingService.GetCoverageReportAsync(tenantId, startDate, endDate);
                return Ok(report);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting coverage report for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get coverage report", error = ex.Message });
            }
        }

        [HttpGet("metrics")]
        public async Task<ActionResult<List<ShiftMetrics>>> GetShiftMetrics(
            [FromQuery] string tenantId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                var metrics = await _shiftSchedulingService.GetShiftMetricsAsync(tenantId, startDate, endDate);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting shift metrics for tenant {TenantId}", tenantId);
                return BadRequest(new { message = "Failed to get shift metrics", error = ex.Message });
            }
        }
    }
}
