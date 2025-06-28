using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using AttendancePlatform.Shared.Infrastructure.Services;

namespace AttendancePlatform.Shared.Infrastructure.Security;

public interface IEnhancedRoleBasedAccessControlService
{
    Task<bool> HasPermissionAsync(ClaimsPrincipal user, string resource, string action);
    Task<bool> HasRoleAsync(ClaimsPrincipal user, string role);
    Task<List<string>> GetUserPermissionsAsync(Guid userId);
    Task<List<string>> GetUserRolesAsync(Guid userId);
    Task<bool> CanAccessTenantResourceAsync(ClaimsPrincipal user, Guid tenantId, string resource);
    Task<RbacEvaluationResult> EvaluateAccessAsync(ClaimsPrincipal user, RbacRequest request);
}

public class EnhancedRoleBasedAccessControlService : IEnhancedRoleBasedAccessControlService
{
    private readonly ILogger<EnhancedRoleBasedAccessControlService> _logger;
    private readonly ICacheService _cacheService;
    private readonly IAuthorizationService _authorizationService;

    public EnhancedRoleBasedAccessControlService(
        ILogger<EnhancedRoleBasedAccessControlService> logger,
        ICacheService cacheService,
        IAuthorizationService authorizationService)
    {
        _logger = logger;
        _cacheService = cacheService;
        _authorizationService = authorizationService;
    }

    public async Task<bool> HasPermissionAsync(ClaimsPrincipal user, string resource, string action)
    {
        try
        {
            var userId = GetUserId(user);
            var tenantId = GetTenantId(user);
            
            var cacheKey = $"rbac_permission_{userId}_{tenantId}_{resource}_{action}";
            var cachedResult = await _cacheService.GetAsync<bool?>(cacheKey);
            
            if (cachedResult.HasValue)
            {
                return cachedResult.Value;
            }

            var hasPermission = await EvaluatePermissionAsync(user, resource, action);
            
            await _cacheService.SetAsync(cacheKey, hasPermission, TimeSpan.FromMinutes(15));
            
            _logger.LogInformation("Permission check: User {UserId} {Action} access to {Resource}: {Result}", 
                userId, hasPermission ? "granted" : "denied", resource, hasPermission);
            
            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission for user {UserId} on resource {Resource}", 
                GetUserId(user), resource);
            return false;
        }
    }

    public async Task<bool> HasRoleAsync(ClaimsPrincipal user, string role)
    {
        try
        {
            var userId = GetUserId(user);
            var userRoles = await GetUserRolesAsync(userId);
            
            var hasRole = userRoles.Contains(role, StringComparer.OrdinalIgnoreCase);
            
            _logger.LogDebug("Role check: User {UserId} {Action} role {Role}", 
                userId, hasRole ? "has" : "does not have", role);
            
            return hasRole;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {Role} for user {UserId}", role, GetUserId(user));
            return false;
        }
    }

    public async Task<List<string>> GetUserPermissionsAsync(Guid userId)
    {
        try
        {
            var cacheKey = $"rbac_user_permissions_{userId}";
            var cachedPermissions = await _cacheService.GetAsync<List<string>>(cacheKey);
            
            if (cachedPermissions != null)
            {
                return cachedPermissions;
            }

            var permissions = await LoadUserPermissionsFromDatabase(userId);
            
            await _cacheService.SetAsync(cacheKey, permissions, TimeSpan.FromMinutes(30));
            
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        try
        {
            var cacheKey = $"rbac_user_roles_{userId}";
            var cachedRoles = await _cacheService.GetAsync<List<string>>(cacheKey);
            
            if (cachedRoles != null)
            {
                return cachedRoles;
            }

            var roles = await LoadUserRolesFromDatabase(userId);
            
            await _cacheService.SetAsync(cacheKey, roles, TimeSpan.FromMinutes(30));
            
            return roles;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return new List<string>();
        }
    }

    public async Task<bool> CanAccessTenantResourceAsync(ClaimsPrincipal user, Guid tenantId, string resource)
    {
        try
        {
            var userTenantId = GetTenantId(user);
            
            if (userTenantId != tenantId)
            {
                _logger.LogWarning("Cross-tenant access attempt: User {UserId} from tenant {UserTenantId} trying to access resource {Resource} in tenant {TargetTenantId}", 
                    GetUserId(user), userTenantId, resource, tenantId);
                return false;
            }

            return await HasPermissionAsync(user, resource, "read");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking tenant resource access for user {UserId}", GetUserId(user));
            return false;
        }
    }

    public async Task<RbacEvaluationResult> EvaluateAccessAsync(ClaimsPrincipal user, RbacRequest request)
    {
        try
        {
            var result = new RbacEvaluationResult
            {
                IsAuthorized = false,
                UserId = GetUserId(user),
                TenantId = GetTenantId(user),
                Resource = request.Resource,
                Action = request.Action,
                EvaluatedAt = DateTime.UtcNow
            };

            if (!user.Identity?.IsAuthenticated == true)
            {
                result.DenialReason = "User is not authenticated";
                return result;
            }

            if (request.TenantId.HasValue && !await CanAccessTenantResourceAsync(user, request.TenantId.Value, request.Resource))
            {
                result.DenialReason = "Cross-tenant access denied";
                return result;
            }

            if (!await HasPermissionAsync(user, request.Resource, request.Action))
            {
                result.DenialReason = "Insufficient permissions";
                return result;
            }

            if (request.AdditionalConstraints?.Any() == true)
            {
                foreach (var constraint in request.AdditionalConstraints)
                {
                    if (!await EvaluateConstraintAsync(user, constraint))
                    {
                        result.DenialReason = $"Constraint failed: {constraint.Type}";
                        return result;
                    }
                }
            }

            result.IsAuthorized = true;
            result.GrantedPermissions = await GetUserPermissionsAsync(result.UserId);
            result.GrantedRoles = await GetUserRolesAsync(result.UserId);

            _logger.LogInformation("RBAC evaluation successful for user {UserId} on {Resource}:{Action}", 
                result.UserId, request.Resource, request.Action);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RBAC evaluation for user {UserId}", GetUserId(user));
            return new RbacEvaluationResult
            {
                IsAuthorized = false,
                DenialReason = "Internal error during authorization",
                UserId = GetUserId(user),
                EvaluatedAt = DateTime.UtcNow
            };
        }
    }

    private async Task<bool> EvaluatePermissionAsync(ClaimsPrincipal user, string resource, string action)
    {
        var userId = GetUserId(user);
        var permissions = await GetUserPermissionsAsync(userId);
        
        var requiredPermission = $"{resource}:{action}";
        var wildcardPermission = $"{resource}:*";
        var adminPermission = "*:*";
        
        return permissions.Contains(requiredPermission, StringComparer.OrdinalIgnoreCase) ||
               permissions.Contains(wildcardPermission, StringComparer.OrdinalIgnoreCase) ||
               permissions.Contains(adminPermission, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<List<string>> LoadUserPermissionsFromDatabase(Guid userId)
    {
        var roles = await LoadUserRolesFromDatabase(userId);
        var permissions = new List<string>();

        foreach (var role in roles)
        {
            permissions.AddRange(GetPermissionsForRole(role));
        }

        return permissions.Distinct().ToList();
    }

    private async Task<List<string>> LoadUserRolesFromDatabase(Guid userId)
    {
        await Task.Delay(1); // Simulate async operation
        
        return new List<string> { "Employee" }; // Default role
    }

    private List<string> GetPermissionsForRole(string role)
    {
        return role.ToLowerInvariant() switch
        {
            "superadmin" => new List<string> { "*:*" },
            "admin" => new List<string> 
            { 
                "users:*", "tenants:*", "attendance:*", "reports:*", "settings:*" 
            },
            "manager" => new List<string> 
            { 
                "attendance:read", "attendance:approve", "reports:read", "users:read", "leave:approve" 
            },
            "hr" => new List<string> 
            { 
                "users:*", "attendance:read", "reports:*", "leave:*", "compliance:*" 
            },
            "employee" => new List<string> 
            { 
                "attendance:create", "attendance:read", "profile:update", "leave:create", "leave:read" 
            },
            _ => new List<string>()
        };
    }

    private async Task<bool> EvaluateConstraintAsync(ClaimsPrincipal user, RbacConstraint constraint)
    {
        return constraint.Type.ToLowerInvariant() switch
        {
            "time" => EvaluateTimeConstraint(constraint),
            "location" => await EvaluateLocationConstraintAsync(user, constraint),
            "device" => EvaluateDeviceConstraint(user, constraint),
            _ => true
        };
    }

    private bool EvaluateTimeConstraint(RbacConstraint constraint)
    {
        if (constraint.Value == null) return true;
        
        var now = DateTime.UtcNow;
        var timeRange = constraint.Value.ToString()?.Split('-');
        
        if (timeRange?.Length == 2 && 
            TimeSpan.TryParse(timeRange[0], out var startTime) && 
            TimeSpan.TryParse(timeRange[1], out var endTime))
        {
            var currentTime = now.TimeOfDay;
            return currentTime >= startTime && currentTime <= endTime;
        }
        
        return true;
    }

    private async Task<bool> EvaluateLocationConstraintAsync(ClaimsPrincipal user, RbacConstraint constraint)
    {
        await Task.Delay(1); // Simulate async operation
        return true; // Simplified for now
    }

    private bool EvaluateDeviceConstraint(ClaimsPrincipal user, RbacConstraint constraint)
    {
        return true; // Simplified for now
    }

    private Guid GetUserId(ClaimsPrincipal user)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private Guid GetTenantId(ClaimsPrincipal user)
    {
        var tenantIdClaim = user.FindFirst("tenant_id")?.Value;
        return Guid.TryParse(tenantIdClaim, out var tenantId) ? tenantId : Guid.Empty;
    }
}

public class RbacRequest
{
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public Guid? TenantId { get; set; }
    public List<RbacConstraint>? AdditionalConstraints { get; set; }
}

public class RbacConstraint
{
    public string Type { get; set; } = string.Empty;
    public object? Value { get; set; }
}

public class RbacEvaluationResult
{
    public bool IsAuthorized { get; set; }
    public Guid UserId { get; set; }
    public Guid TenantId { get; set; }
    public string Resource { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? DenialReason { get; set; }
    public List<string> GrantedPermissions { get; set; } = new();
    public List<string> GrantedRoles { get; set; } = new();
    public DateTime EvaluatedAt { get; set; }
}
