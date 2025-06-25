using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AttendancePlatform.Workflow.Api.Services
{
    public class WorkflowTemplateService : IWorkflowTemplateService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<WorkflowTemplateService> _logger;

        public WorkflowTemplateService(AttendancePlatformDbContext context, ILogger<WorkflowTemplateService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<WorkflowTemplateDto> CreateTemplateAsync(CreateWorkflowTemplateRequestDto request)
        {
            try
            {
                var template = new WorkflowTemplate
                {
                    Id = Guid.NewGuid(),
                    TenantId = request.TenantId,
                    WorkflowType = request.WorkflowType,
                    Name = request.Name,
                    Description = request.Description,
                    StepDefinitions = request.StepDefinitions ?? "[]",
                    Steps = request.Steps ?? "[]",
                    StepCount = request.StepCount,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = request.CreatedBy
                };

                _context.WorkflowTemplates.Add(template);
                await _context.SaveChangesAsync();

                return new WorkflowTemplateDto
                {
                    Id = template.Id,
                    TenantId = template.TenantId,
                    WorkflowType = template.WorkflowType,
                    Name = template.Name,
                    Description = template.Description,
                    StepDefinitions = template.StepDefinitions,
                    StepCount = template.StepCount,
                    IsActive = template.IsActive,
                    CreatedAt = template.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating workflow template");
                throw;
            }
        }

        public async Task<List<WorkflowTemplateDto>> GetTemplatesAsync(Guid tenantId, string category)
        {
            try
            {
                var query = _context.WorkflowTemplates
                    .Where(t => t.TenantId == tenantId);

                if (!string.IsNullOrEmpty(category))
                {
                    query = query.Where(t => t.WorkflowType == category);
                }

                var templates = await query
                    .Select(t => new WorkflowTemplateDto
                    {
                        Id = t.Id,
                        TenantId = t.TenantId,
                        WorkflowType = t.WorkflowType,
                        Name = t.Name,
                        Description = t.Description,
                        StepDefinitions = t.StepDefinitions,
                        StepCount = t.StepCount,
                        IsActive = t.IsActive,
                        CreatedAt = t.CreatedAt
                    })
                    .ToListAsync();

                return templates;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow templates for tenant {TenantId}", tenantId);
                throw;
            }
        }

        public async Task<WorkflowTemplateDto> GetTemplateAsync(Guid templateId)
        {
            try
            {
                var template = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template == null)
                    throw new ArgumentException($"Workflow template with ID {templateId} not found");

                return new WorkflowTemplateDto
                {
                    Id = template.Id,
                    TenantId = template.TenantId,
                    WorkflowType = template.WorkflowType,
                    Name = template.Name,
                    Description = template.Description,
                    StepDefinitions = template.StepDefinitions,
                    StepCount = template.StepCount,
                    IsActive = template.IsActive,
                    CreatedAt = template.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow template {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<bool> DeleteTemplateAsync(Guid templateId)
        {
            try
            {
                var template = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template == null)
                    return false;

                _context.WorkflowTemplates.Remove(template);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting workflow template {TemplateId}", templateId);
                throw;
            }
        }

        public async Task<List<WorkflowTemplateCategoryDto>> GetTemplateCategoriesAsync()
        {
            try
            {
                var categories = await _context.WorkflowTemplates
                    .Where(t => t.IsActive)
                    .GroupBy(t => t.WorkflowType)
                    .Select(g => new WorkflowTemplateCategoryDto
                    {
                        Name = g.Key,
                        Count = g.Count(),
                        Description = $"{g.Key} workflow templates"
                    })
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving workflow template categories");
                throw;
            }
        }

        public async Task<WorkflowTemplateDto> UpdateTemplateAsync(Guid templateId, UpdateWorkflowTemplateRequestDto request)
        {
            try
            {
                var template = await _context.WorkflowTemplates
                    .FirstOrDefaultAsync(t => t.Id == templateId);

                if (template == null)
                    throw new ArgumentException($"Workflow template with ID {templateId} not found");

                template.Name = request.Name;
                template.Description = request.Description;
                template.WorkflowType = request.WorkflowType;
                template.StepDefinitions = request.StepDefinitions ?? template.StepDefinitions;
                template.Steps = request.Steps ?? template.Steps;
                template.StepCount = request.StepCount;
                template.IsActive = request.IsActive;
                template.UpdatedAt = DateTime.UtcNow;
                template.UpdatedBy = request.UpdatedBy;

                await _context.SaveChangesAsync();

                return new WorkflowTemplateDto
                {
                    Id = template.Id,
                    TenantId = template.TenantId,
                    WorkflowType = template.WorkflowType,
                    Name = template.Name,
                    Description = template.Description,
                    StepDefinitions = template.StepDefinitions,
                    StepCount = template.StepCount,
                    IsActive = template.IsActive,
                    CreatedAt = template.CreatedAt
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating workflow template {TemplateId}", templateId);
                throw;
            }
        }
    }
}
