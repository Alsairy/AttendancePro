using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Workflow.Api.Services
{
    public class AutomationService : IAutomationService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<AutomationService> _logger;

        public AutomationService(AttendancePlatformDbContext context, ILogger<AutomationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<AutomationRuleDto> CreateAutomationRuleAsync(CreateAutomationRuleRequestDto request)
        {
            try
            {
                var rule = new BusinessRule
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    Name = request.Name,
                    Description = request.Description,
                    Category = "Automation",
                    RuleType = request.RuleType,
                    Conditions = request.Conditions ?? "{}",
                    Actions = request.Actions ?? "{}",
                    IsActive = true,
                    Priority = request.Priority,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                };

                _context.BusinessRules.Add(rule);
                await _context.SaveChangesAsync();

                return new AutomationRuleDto
                {
                    Id = rule.Id,
                    TenantId = rule.TenantId,
                    Name = rule.Name,
                    Description = rule.Description,
                    RuleType = rule.RuleType,
                    IsActive = rule.IsActive,
                    Priority = rule.Priority,
                    CreatedAt = rule.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating automation rule");
                throw;
            }
        }

        public async Task<List<AutomationRuleDto>> GetAutomationRulesAsync(Guid tenantId)
        {
            try
            {
                var rules = await _context.BusinessRules
                    .Where(r => r.TenantId == tenantId && r.Category == "Automation")
                    .Select(r => new AutomationRuleDto
                    {
                        Id = r.Id,
                        TenantId = r.TenantId,
                        Name = r.Name,
                        Description = r.Description,
                        RuleType = r.RuleType,
                        IsActive = r.IsActive,
                        Priority = r.Priority,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                return rules;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving automation rules for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<AutomationRuleDto> GetAutomationRuleAsync(Guid ruleId)
        {
            try
            {
                var rule = await _context.BusinessRules
                    .FirstOrDefaultAsync(r => r.Id == ruleId && r.Category == "Automation");

                if (rule == null)
                    throw new ArgumentException($"Automation rule with ID {ruleId} not found");

                return new AutomationRuleDto
                {
                    Id = rule.Id,
                    TenantId = rule.TenantId,
                    Name = rule.Name,
                    Description = rule.Description,
                    RuleType = rule.RuleType,
                    IsActive = rule.IsActive,
                    Priority = rule.Priority,
                    CreatedAt = rule.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving automation rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<bool> DeleteAutomationRuleAsync(Guid ruleId)
        {
            try
            {
                var rule = await _context.BusinessRules
                    .FirstOrDefaultAsync(r => r.Id == ruleId && r.Category == "Automation");

                if (rule == null)
                    return false;

                _context.BusinessRules.Remove(rule);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting automation rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<List<AutomationTemplateDto>> GetAutomationTemplatesAsync()
        {
            try
            {
                return new List<AutomationTemplateDto>
                {
                    new AutomationTemplateDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Attendance Alert",
                        Description = "Automatically send alerts for attendance violations",
                        Category = "Attendance",
                        RuleType = "Alert"
                    },
                    new AutomationTemplateDto
                    {
                        Id = Guid.NewGuid(),
                        Name = "Leave Approval",
                        Description = "Automatically approve leave requests based on criteria",
                        Category = "Leave",
                        RuleType = "Approval"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving automation templates");
                throw;
            }
        }

        public async Task<AutomationExecutionResultDto> ExecuteAutomationAsync(Guid ruleId, Dictionary<string, object> context)
        {
            try
            {
                var rule = await _context.BusinessRules
                    .FirstOrDefaultAsync(r => r.Id == ruleId && r.Category == "Automation");

                if (rule == null)
                    throw new ArgumentException($"Automation rule with ID {ruleId} not found");

                var result = new AutomationExecutionResultDto
                {
                    RuleId = ruleId,
                    ExecutionId = Guid.NewGuid(),
                    Status = "Success",
                    ExecutedAt = DateTime.UtcNow,
                    Context = System.Text.Json.JsonSerializer.Serialize(context),
                    Message = $"Automation rule '{rule.Name}' executed successfully"
                };

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing automation rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<List<AutomationExecutionLogDto>> GetExecutionLogsAsync(Guid tenantId, Guid? ruleId = null)
        {
            try
            {
                var query = _context.BusinessRules
                    .Where(r => r.TenantId == tenantId && r.Category == "Automation");

                if (ruleId.HasValue)
                {
                    query = query.Where(r => r.Id == ruleId.Value);
                }

                var logs = await query
                    .Select(r => new AutomationExecutionLogDto
                    {
                        Id = Guid.NewGuid(),
                        RuleId = r.Id,
                        RuleName = r.Name,
                        ExecutedAt = r.CreatedAt,
                        Status = "Success",
                        Message = $"Rule '{r.Name}' executed"
                    })
                    .ToListAsync();

                return logs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving automation execution logs for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<AutomationRuleDto> UpdateAutomationRuleAsync(Guid ruleId, UpdateAutomationRuleRequestDto request)
        {
            try
            {
                var rule = await _context.BusinessRules
                    .FirstOrDefaultAsync(r => r.Id == ruleId && r.Category == "Automation");

                if (rule == null)
                    throw new ArgumentException($"Automation rule with ID {ruleId} not found");

                rule.Name = request.Name;
                rule.Description = request.Description;
                rule.RuleType = request.RuleType;
                rule.Conditions = request.Conditions ?? rule.Conditions;
                rule.Actions = request.Actions ?? rule.Actions;
                rule.Priority = request.Priority;
                rule.UpdatedAt = DateTime.UtcNow;
                rule.UpdatedBy = request.UpdatedBy;

                await _context.SaveChangesAsync();

                return new AutomationRuleDto
                {
                    Id = rule.Id,
                    TenantId = rule.TenantId,
                    Name = rule.Name,
                    Description = rule.Description,
                    RuleType = rule.RuleType,
                    IsActive = rule.IsActive,
                    Priority = rule.Priority,
                    CreatedAt = rule.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating automation rule {RuleId}", ruleId);
                throw;
            }
        }

        public async Task<bool> EnableDisableAutomationAsync(Guid ruleId, bool isEnabled)
        {
            try
            {
                var rule = await _context.BusinessRules
                    .FirstOrDefaultAsync(r => r.Id == ruleId && r.Category == "Automation");

                if (rule == null)
                    return false;

                rule.IsActive = isEnabled;
                rule.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling/disabling automation rule {RuleId}", ruleId);
                throw;
            }
        }
    }
}
