using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class ScimService : IScimService
    {
        private readonly ILogger<ScimService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public ScimService(ILogger<ScimService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<ScimUserDto> CreateUserAsync(ScimUserDto scimUser)
        {
            try
            {
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = scimUser.UserName ?? string.Empty,
                    Email = scimUser.Emails?.FirstOrDefault()?.Value,
                    FirstName = scimUser.Name?.GivenName,
                    LastName = scimUser.Name?.FamilyName,
                    IsActive = scimUser.Active,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                scimUser.Id = user.Id.ToString();
                scimUser.Meta = new ScimMetaDto
                {
                    ResourceType = "User",
                    Created = user.CreatedAt,
                    LastModified = user.UpdatedAt,
                    Location = $"/scim/v2/Users/{user.Id}",
                    Version = "1"
                };

                _logger.LogInformation("SCIM user created successfully: {UserId}", user.Id);
                return scimUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create SCIM user");
                throw;
            }
        }

        public async Task<ScimUserDto> GetUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                if (user == null) return null;

                return MapUserToScimDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get SCIM user: {UserId}", userId);
                throw;
            }
        }

        public async Task<ScimUserDto> UpdateUserAsync(string userId, ScimUserDto scimUser)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                if (user == null) return null;

                user.Username = scimUser.UserName ?? string.Empty;
                user.Email = scimUser.Emails?.FirstOrDefault()?.Value;
                user.FirstName = scimUser.Name?.GivenName;
                user.LastName = scimUser.Name?.FamilyName;
                user.IsActive = scimUser.Active;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("SCIM user updated successfully: {UserId}", userId);
                return MapUserToScimDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update SCIM user: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                if (user == null) return false;

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("SCIM user deactivated successfully: {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete SCIM user: {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<ScimUserDto>> GetUsersAsync(int startIndex = 1, int count = 100, string filter = null)
        {
            try
            {
                var query = _context.Users.AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    if (filter.Contains("userName"))
                    {
                        var userName = ExtractFilterValue(filter, "userName");
                        query = query.Where(u => u.Username.Contains(userName));
                    }
                }

                var users = await query
                    .Skip(startIndex - 1)
                    .Take(count)
                    .ToListAsync();

                return users.Select(MapUserToScimDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get SCIM users");
                throw;
            }
        }

        public async Task<ScimGroupDto> CreateGroupAsync(ScimGroupDto group)
        {
            try
            {
                var role = new Role
                {
                    Id = Guid.NewGuid(),
                    Name = group.DisplayName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Roles.Add(role);
                await _context.SaveChangesAsync();

                group.Id = role.Id.ToString();
                group.Meta = new ScimMetaDto
                {
                    ResourceType = "Group",
                    Created = role.CreatedAt,
                    LastModified = role.UpdatedAt,
                    Location = $"/scim/v2/Groups/{role.Id}",
                    Version = "1"
                };

                _logger.LogInformation("SCIM group created successfully: {GroupId}", role.Id);
                return group;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create SCIM group");
                throw;
            }
        }

        public async Task<ScimGroupDto> GetGroupAsync(string groupId)
        {
            try
            {
                var role = await _context.Roles.FindAsync(Guid.Parse(groupId));
                if (role == null) return null;

                return MapRoleToScimDto(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get SCIM group: {GroupId}", groupId);
                throw;
            }
        }

        public async Task<ScimGroupDto> UpdateGroupAsync(string groupId, ScimGroupDto group)
        {
            try
            {
                var role = await _context.Roles.FindAsync(Guid.Parse(groupId));
                if (role == null) return null;

                role.Name = group.DisplayName;
                role.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("SCIM group updated successfully: {GroupId}", groupId);
                return MapRoleToScimDto(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update SCIM group: {GroupId}", groupId);
                throw;
            }
        }

        public async Task<bool> DeleteGroupAsync(string groupId)
        {
            try
            {
                var role = await _context.Roles.FindAsync(Guid.Parse(groupId));
                if (role == null) return false;

                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();

                _logger.LogInformation("SCIM group deleted successfully: {GroupId}", groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete SCIM group: {GroupId}", groupId);
                throw;
            }
        }

        public async Task<IEnumerable<ScimGroupDto>> GetGroupsAsync(int startIndex = 1, int count = 100, string filter = null)
        {
            try
            {
                var query = _context.Roles.AsQueryable();

                if (!string.IsNullOrEmpty(filter))
                {
                    if (filter.Contains("displayName"))
                    {
                        var displayName = ExtractFilterValue(filter, "displayName");
                        query = query.Where(r => r.Name.Contains(displayName));
                    }
                }

                var roles = await query
                    .Skip(startIndex - 1)
                    .Take(count)
                    .ToListAsync();

                return roles.Select(MapRoleToScimDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get SCIM groups");
                throw;
            }
        }

        public async Task<bool> AddUserToGroupAsync(string groupId, string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                var role = await _context.Roles.FindAsync(Guid.Parse(groupId));

                if (user == null || role == null) return false;

                user.RoleId = role.Id;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} added to group {GroupId}", userId, groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user to group: {UserId}, {GroupId}", userId, groupId);
                throw;
            }
        }

        public async Task<bool> RemoveUserFromGroupAsync(string groupId, string userId)
        {
            try
            {
                var user = await _context.Users.FindAsync(Guid.Parse(userId));
                if (user == null) return false;

                user.RoleId = null;
                user.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("User {UserId} removed from group {GroupId}", userId, groupId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user from group: {UserId}, {GroupId}", userId, groupId);
                throw;
            }
        }

        public async Task<ScimResourceTypeDto> GetResourceTypeAsync(string resourceType)
        {
            await Task.CompletedTask;
            
            return resourceType.ToLower() switch
            {
                "user" => new ScimResourceTypeDto
                {
                    Id = "User",
                    Name = "User",
                    Description = "User Account",
                    Endpoint = "/Users",
                    Schema = "urn:ietf:params:scim:schemas:core:2.0:User",
                    SchemaExtensions = new List<string> { "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" }
                },
                "group" => new ScimResourceTypeDto
                {
                    Id = "Group",
                    Name = "Group",
                    Description = "Group",
                    Endpoint = "/Groups",
                    Schema = "urn:ietf:params:scim:schemas:core:2.0:Group"
                },
                _ => null
            };
        }

        public async Task<IEnumerable<ScimResourceTypeDto>> GetResourceTypesAsync()
        {
            await Task.CompletedTask;
            
            return new List<ScimResourceTypeDto>
            {
                await GetResourceTypeAsync("User"),
                await GetResourceTypeAsync("Group")
            };
        }

        public async Task<ScimSchemaDto> GetSchemaAsync(string schemaId)
        {
            await Task.CompletedTask;
            
            return schemaId switch
            {
                "urn:ietf:params:scim:schemas:core:2.0:User" => GetUserSchema(),
                "urn:ietf:params:scim:schemas:core:2.0:Group" => GetGroupSchema(),
                "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User" => GetEnterpriseUserSchema(),
                _ => null
            };
        }

        public async Task<IEnumerable<ScimSchemaDto>> GetSchemasAsync()
        {
            await Task.CompletedTask;
            
            return new List<ScimSchemaDto>
            {
                GetUserSchema(),
                GetGroupSchema(),
                GetEnterpriseUserSchema()
            };
        }

        public async Task<ScimServiceProviderConfigDto> GetServiceProviderConfigAsync()
        {
            await Task.CompletedTask;
            
            return new ScimServiceProviderConfigDto
            {
                Bulk = new ScimBulkDto { Supported = false, MaxOperations = 0, MaxPayloadSize = 0 },
                Filter = new ScimFilterDto { Supported = true, MaxResults = 200 },
                ChangePassword = new ScimChangePasswordDto { Supported = true },
                Sort = new ScimSortDto { Supported = false },
                Patch = new ScimPatchDto { Supported = true },
                ETag = new ScimETagDto { Supported = false },
                AuthenticationSchemes = new List<ScimAuthenticationSchemeDto>
                {
                    new ScimAuthenticationSchemeDto
                    {
                        Type = "oauthbearertoken",
                        Name = "OAuth Bearer Token",
                        Description = "Authentication scheme using the OAuth Bearer Token Standard"
                    }
                }
            };
        }

        private ScimUserDto MapUserToScimDto(User user)
        {
            return new ScimUserDto
            {
                Id = user.Id.ToString(),
                UserName = user.Username ?? string.Empty,
                Name = new ScimNameDto
                {
                    GivenName = user.FirstName,
                    FamilyName = user.LastName,
                    Formatted = $"{user.FirstName} {user.LastName}"
                },
                DisplayName = $"{user.FirstName} {user.LastName}",
                Active = user.IsActive,
                Emails = new List<ScimEmailDto>
                {
                    new ScimEmailDto { Value = user.Email, Primary = true, Type = "work" }
                },
                Meta = new ScimMetaDto
                {
                    ResourceType = "User",
                    Created = user.CreatedAt,
                    LastModified = user.UpdatedAt,
                    Location = $"/scim/v2/Users/{user.Id}",
                    Version = "1"
                }
            };
        }

        private ScimGroupDto MapRoleToScimDto(Role role)
        {
            return new ScimGroupDto
            {
                Id = role.Id.ToString(),
                DisplayName = role.Name,
                Meta = new ScimMetaDto
                {
                    ResourceType = "Group",
                    Created = role.CreatedAt,
                    LastModified = role.UpdatedAt,
                    Location = $"/scim/v2/Groups/{role.Id}",
                    Version = "1"
                }
            };
        }

        private string ExtractFilterValue(string filter, string attribute)
        {
            var parts = filter.Split(' ');
            var index = Array.FindIndex(parts, p => p.Contains(attribute));
            return index >= 0 && index + 2 < parts.Length ? parts[index + 2].Trim('"') : string.Empty;
        }

        private ScimSchemaDto GetUserSchema()
        {
            return new ScimSchemaDto
            {
                Id = "urn:ietf:params:scim:schemas:core:2.0:User",
                Name = "User",
                Description = "User Account",
                Attributes = new List<ScimAttributeDto>
                {
                    new ScimAttributeDto { Name = "userName", Type = "string", Required = true, Uniqueness = "server" },
                    new ScimAttributeDto { Name = "name", Type = "complex", Required = false },
                    new ScimAttributeDto { Name = "displayName", Type = "string", Required = false },
                    new ScimAttributeDto { Name = "active", Type = "boolean", Required = false }
                }
            };
        }

        private ScimSchemaDto GetGroupSchema()
        {
            return new ScimSchemaDto
            {
                Id = "urn:ietf:params:scim:schemas:core:2.0:Group",
                Name = "Group",
                Description = "Group",
                Attributes = new List<ScimAttributeDto>
                {
                    new ScimAttributeDto { Name = "displayName", Type = "string", Required = true },
                    new ScimAttributeDto { Name = "members", Type = "complex", MultiValued = true, Required = false }
                }
            };
        }

        private ScimSchemaDto GetEnterpriseUserSchema()
        {
            return new ScimSchemaDto
            {
                Id = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
                Name = "EnterpriseUser",
                Description = "Enterprise User",
                Attributes = new List<ScimAttributeDto>
                {
                    new ScimAttributeDto { Name = "employeeNumber", Type = "string", Required = false },
                    new ScimAttributeDto { Name = "department", Type = "string", Required = false },
                    new ScimAttributeDto { Name = "manager", Type = "complex", Required = false }
                }
            };
        }
    }
}
