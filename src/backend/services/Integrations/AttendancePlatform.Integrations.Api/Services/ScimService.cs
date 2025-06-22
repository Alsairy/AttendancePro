using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;
using System.Text.Json;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IScimService
    {
        Task<ScimUser?> GetUserAsync(string id);
        Task<ScimUserListResponse> GetUsersAsync(string? filter = null, int startIndex = 1, int count = 20);
        Task<ScimUser> CreateUserAsync(ScimUser user);
        Task<ScimUser?> UpdateUserAsync(string id, ScimUser user);
        Task<bool> DeleteUserAsync(string id);
        Task<ScimGroup?> GetGroupAsync(string id);
        Task<ScimGroupListResponse> GetGroupsAsync(string? filter = null, int startIndex = 1, int count = 20);
        Task<ScimGroup> CreateGroupAsync(ScimGroup group);
        Task<ScimGroup?> UpdateGroupAsync(string id, ScimGroup group);
        Task<bool> DeleteGroupAsync(string id);
        Task<ScimServiceProviderConfig> GetServiceProviderConfigAsync();
        Task<ScimResourceTypes> GetResourceTypesAsync();
        Task<ScimSchemas> GetSchemasAsync();
    }

    public class ScimService : IScimService
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly ILogger<ScimService> _logger;
        private readonly ITenantContext _tenantContext;

        public ScimService(
            AttendancePlatformDbContext context,
            ILogger<ScimService> logger,
            ITenantContext tenantContext)
        {
            _context = context;
            _logger = logger;
            _tenantContext = tenantContext;
        }

        public async Task<ScimUser?> GetUserAsync(string id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Id.ToString() == id && u.TenantId == _tenantContext.TenantId);

            return user != null ? MapToScimUser(user) : null;
        }

        public async Task<ScimUserListResponse> GetUsersAsync(string? filter = null, int startIndex = 1, int count = 20)
        {
            var query = _context.Users
                .Include(u => u.Roles)
                .Where(u => u.TenantId == _tenantContext.TenantId);

            // Apply SCIM filter if provided
            if (!string.IsNullOrEmpty(filter))
            {
                query = ApplyUserFilter(query, filter);
            }

            var totalResults = await query.CountAsync();
            var users = await query
                .Skip(startIndex - 1)
                .Take(count)
                .ToListAsync();

            return new ScimUserListResponse
            {
                Schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                TotalResults = totalResults,
                StartIndex = startIndex,
                ItemsPerPage = count,
                Resources = users.Select(MapToScimUser).ToArray()
            };
        }

        public async Task<ScimUser> CreateUserAsync(ScimUser scimUser)
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                UserName = scimUser.UserName,
                Email = scimUser.Emails?.FirstOrDefault()?.Value ?? "",
                FirstName = scimUser.Name?.GivenName ?? "",
                LastName = scimUser.Name?.FamilyName ?? "",
                PhoneNumber = scimUser.PhoneNumbers?.FirstOrDefault()?.Value,
                IsActive = scimUser.Active,
                CreatedAt = DateTime.UtcNow,
                ExternalId = scimUser.ExternalId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"SCIM user created: {user.Id}");

            return MapToScimUser(user);
        }

        public async Task<ScimUser?> UpdateUserAsync(string id, ScimUser scimUser)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == id && u.TenantId == _tenantContext.TenantId);

            if (user == null)
                return null;

            user.UserName = scimUser.UserName ?? user.UserName;
            user.Email = scimUser.Emails?.FirstOrDefault()?.Value ?? user.Email;
            user.FirstName = scimUser.Name?.GivenName ?? user.FirstName;
            user.LastName = scimUser.Name?.FamilyName ?? user.LastName;
            user.PhoneNumber = scimUser.PhoneNumbers?.FirstOrDefault()?.Value ?? user.PhoneNumber;
            user.IsActive = scimUser.Active;
            user.UpdatedAt = DateTime.UtcNow;
            user.ExternalId = scimUser.ExternalId ?? user.ExternalId;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"SCIM user updated: {user.Id}");

            return MapToScimUser(user);
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id.ToString() == id && u.TenantId == _tenantContext.TenantId);

            if (user == null)
                return false;

            user.IsDeleted = true;
            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"SCIM user deleted: {user.Id}");

            return true;
        }

        public async Task<ScimGroup?> GetGroupAsync(string id)
        {
            var role = await _context.Roles
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Id.ToString() == id && r.TenantId == _tenantContext.TenantId);

            return role != null ? MapToScimGroup(role) : null;
        }

        public async Task<ScimGroupListResponse> GetGroupsAsync(string? filter = null, int startIndex = 1, int count = 20)
        {
            var query = _context.Roles
                .Include(r => r.Users)
                .Where(r => r.TenantId == _tenantContext.TenantId);

            // Apply SCIM filter if provided
            if (!string.IsNullOrEmpty(filter))
            {
                query = ApplyGroupFilter(query, filter);
            }

            var totalResults = await query.CountAsync();
            var roles = await query
                .Skip(startIndex - 1)
                .Take(count)
                .ToListAsync();

            return new ScimGroupListResponse
            {
                Schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                TotalResults = totalResults,
                StartIndex = startIndex,
                ItemsPerPage = count,
                Resources = roles.Select(MapToScimGroup).ToArray()
            };
        }

        public async Task<ScimGroup> CreateGroupAsync(ScimGroup scimGroup)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                TenantId = _tenantContext.TenantId,
                Name = scimGroup.DisplayName,
                Description = scimGroup.DisplayName,
                CreatedAt = DateTime.UtcNow
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"SCIM group created: {role.Id}");

            return MapToScimGroup(role);
        }

        public async Task<ScimGroup?> UpdateGroupAsync(string id, ScimGroup scimGroup)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id.ToString() == id && r.TenantId == _tenantContext.TenantId);

            if (role == null)
                return null;

            role.Name = scimGroup.DisplayName ?? role.Name;
            role.Description = scimGroup.DisplayName ?? role.Description;
            role.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation($"SCIM group updated: {role.Id}");

            return MapToScimGroup(role);
        }

        public async Task<bool> DeleteGroupAsync(string id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id.ToString() == id && r.TenantId == _tenantContext.TenantId);

            if (role == null)
                return false;

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"SCIM group deleted: {role.Id}");

            return true;
        }

        public async Task<ScimServiceProviderConfig> GetServiceProviderConfigAsync()
        {
            return await Task.FromResult(new ScimServiceProviderConfig
            {
                Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ServiceProviderConfig" },
                DocumentationUri = "https://docs.hudu.sa/scim",
                Patch = new ScimSupported { Supported = true },
                Bulk = new ScimBulkSupported 
                { 
                    Supported = true, 
                    MaxOperations = 1000, 
                    MaxPayloadSize = 1048576 
                },
                Filter = new ScimFilterSupported 
                { 
                    Supported = true, 
                    MaxResults = 200 
                },
                ChangePassword = new ScimSupported { Supported = true },
                Sort = new ScimSupported { Supported = true },
                Etag = new ScimSupported { Supported = false },
                AuthenticationSchemes = new[]
                {
                    new ScimAuthenticationScheme
                    {
                        Type = "oauthbearertoken",
                        Name = "OAuth Bearer Token",
                        Description = "Authentication scheme using the OAuth Bearer Token Standard",
                        SpecUri = "http://www.rfc-editor.org/info/rfc6750",
                        DocumentationUri = "https://docs.hudu.sa/scim/auth"
                    }
                }
            });
        }

        public async Task<ScimResourceTypes> GetResourceTypesAsync()
        {
            return await Task.FromResult(new ScimResourceTypes
            {
                Schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                TotalResults = 2,
                Resources = new[]
                {
                    new ScimResourceType
                    {
                        Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ResourceType" },
                        Id = "User",
                        Name = "User",
                        Endpoint = "/Users",
                        Description = "User Account",
                        Schema = "urn:ietf:params:scim:schemas:core:2.0:User",
                        SchemaExtensions = new[]
                        {
                            new ScimSchemaExtension
                            {
                                Schema = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
                                Required = false
                            }
                        }
                    },
                    new ScimResourceType
                    {
                        Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:ResourceType" },
                        Id = "Group",
                        Name = "Group",
                        Endpoint = "/Groups",
                        Description = "Group",
                        Schema = "urn:ietf:params:scim:schemas:core:2.0:Group"
                    }
                }
            });
        }

        public async Task<ScimSchemas> GetSchemasAsync()
        {
            return await Task.FromResult(new ScimSchemas
            {
                Schemas = new[] { "urn:ietf:params:scim:api:messages:2.0:ListResponse" },
                TotalResults = 3,
                Resources = new[]
                {
                    new ScimSchema
                    {
                        Id = "urn:ietf:params:scim:schemas:core:2.0:User",
                        Name = "User",
                        Description = "User Account"
                    },
                    new ScimSchema
                    {
                        Id = "urn:ietf:params:scim:schemas:core:2.0:Group",
                        Name = "Group",
                        Description = "Group"
                    },
                    new ScimSchema
                    {
                        Id = "urn:ietf:params:scim:schemas:extension:enterprise:2.0:User",
                        Name = "EnterpriseUser",
                        Description = "Enterprise User"
                    }
                }
            });
        }

        private IQueryable<User> ApplyUserFilter(IQueryable<User> query, string filter)
        {
            // Simple SCIM filter parsing - in production, use a proper SCIM filter parser
            if (filter.Contains("userName eq"))
            {
                var userName = ExtractFilterValue(filter, "userName eq");
                query = query.Where(u => u.UserName == userName);
            }
            else if (filter.Contains("emails.value eq"))
            {
                var email = ExtractFilterValue(filter, "emails.value eq");
                query = query.Where(u => u.Email == email);
            }
            else if (filter.Contains("active eq"))
            {
                var active = ExtractFilterValue(filter, "active eq") == "true";
                query = query.Where(u => u.IsActive == active);
            }

            return query;
        }

        private IQueryable<Role> ApplyGroupFilter(IQueryable<Role> query, string filter)
        {
            if (filter.Contains("displayName eq"))
            {
                var displayName = ExtractFilterValue(filter, "displayName eq");
                query = query.Where(r => r.Name == displayName);
            }

            return query;
        }

        private string ExtractFilterValue(string filter, string attribute)
        {
            var index = filter.IndexOf(attribute) + attribute.Length;
            var value = filter.Substring(index).Trim();
            return value.Trim('"', '\'');
        }

        private ScimUser MapToScimUser(User user)
        {
            return new ScimUser
            {
                Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:User" },
                Id = user.Id.ToString(),
                ExternalId = user.ExternalId,
                UserName = user.UserName,
                Active = user.IsActive,
                Name = new ScimName
                {
                    GivenName = user.FirstName,
                    FamilyName = user.LastName,
                    Formatted = $"{user.FirstName} {user.LastName}"
                },
                Emails = new[]
                {
                    new ScimEmail
                    {
                        Value = user.Email,
                        Type = "work",
                        Primary = true
                    }
                },
                PhoneNumbers = !string.IsNullOrEmpty(user.PhoneNumber) ? new[]
                {
                    new ScimPhoneNumber
                    {
                        Value = user.PhoneNumber,
                        Type = "work"
                    }
                } : null,
                Meta = new ScimMeta
                {
                    ResourceType = "User",
                    Created = user.CreatedAt,
                    LastModified = user.UpdatedAt ?? user.CreatedAt,
                    Location = $"/scim/v2/Users/{user.Id}",
                    Version = $"W/\"{user.UpdatedAt?.Ticks ?? user.CreatedAt.Ticks}\""
                }
            };
        }

        private ScimGroup MapToScimGroup(Role role)
        {
            return new ScimGroup
            {
                Schemas = new[] { "urn:ietf:params:scim:schemas:core:2.0:Group" },
                Id = role.Id.ToString(),
                DisplayName = role.Name,
                Members = role.Users?.Select(u => new ScimMember
                {
                    Value = u.Id.ToString(),
                    Ref = $"/scim/v2/Users/{u.Id}",
                    Type = "User"
                }).ToArray(),
                Meta = new ScimMeta
                {
                    ResourceType = "Group",
                    Created = role.CreatedAt,
                    LastModified = role.UpdatedAt ?? role.CreatedAt,
                    Location = $"/scim/v2/Groups/{role.Id}",
                    Version = $"W/\"{role.UpdatedAt?.Ticks ?? role.CreatedAt.Ticks}\""
                }
            };
        }
    }

    // SCIM 2.0 Models
    public class ScimUser
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public string Id { get; set; } = string.Empty;
        public string? ExternalId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public ScimName? Name { get; set; }
        public string? DisplayName { get; set; }
        public string? NickName { get; set; }
        public string? ProfileUrl { get; set; }
        public string? Title { get; set; }
        public string? UserType { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? Locale { get; set; }
        public string? Timezone { get; set; }
        public bool Active { get; set; } = true;
        public string? Password { get; set; }
        public ScimEmail[]? Emails { get; set; }
        public ScimPhoneNumber[]? PhoneNumbers { get; set; }
        public ScimIms[]? Ims { get; set; }
        public ScimPhoto[]? Photos { get; set; }
        public ScimAddress[]? Addresses { get; set; }
        public ScimGroup[]? Groups { get; set; }
        public ScimEntitlement[]? Entitlements { get; set; }
        public ScimRole[]? Roles { get; set; }
        public ScimX509Certificate[]? X509Certificates { get; set; }
        public ScimMeta? Meta { get; set; }
    }

    public class ScimGroup
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public string Id { get; set; } = string.Empty;
        public string? ExternalId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public ScimMember[]? Members { get; set; }
        public ScimMeta? Meta { get; set; }
    }

    public class ScimName
    {
        public string? Formatted { get; set; }
        public string? FamilyName { get; set; }
        public string? GivenName { get; set; }
        public string? MiddleName { get; set; }
        public string? HonorificPrefix { get; set; }
        public string? HonorificSuffix { get; set; }
    }

    public class ScimEmail
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimPhoneNumber
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimIms
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimPhoto
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimAddress
    {
        public string? Formatted { get; set; }
        public string? StreetAddress { get; set; }
        public string? Locality { get; set; }
        public string? Region { get; set; }
        public string? PostalCode { get; set; }
        public string? Country { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimEntitlement
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimRole
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimX509Certificate
    {
        public string Value { get; set; } = string.Empty;
        public string? Display { get; set; }
        public string? Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimMember
    {
        public string Value { get; set; } = string.Empty;
        public string? Ref { get; set; }
        public string? Type { get; set; }
        public string? Display { get; set; }
    }

    public class ScimMeta
    {
        public string? ResourceType { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string? Location { get; set; }
        public string? Version { get; set; }
    }

    public class ScimUserListResponse
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public int TotalResults { get; set; }
        public int StartIndex { get; set; }
        public int ItemsPerPage { get; set; }
        public ScimUser[] Resources { get; set; } = Array.Empty<ScimUser>();
    }

    public class ScimGroupListResponse
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public int TotalResults { get; set; }
        public int StartIndex { get; set; }
        public int ItemsPerPage { get; set; }
        public ScimGroup[] Resources { get; set; } = Array.Empty<ScimGroup>();
    }

    public class ScimServiceProviderConfig
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public string? DocumentationUri { get; set; }
        public ScimSupported? Patch { get; set; }
        public ScimBulkSupported? Bulk { get; set; }
        public ScimFilterSupported? Filter { get; set; }
        public ScimSupported? ChangePassword { get; set; }
        public ScimSupported? Sort { get; set; }
        public ScimSupported? Etag { get; set; }
        public ScimAuthenticationScheme[]? AuthenticationSchemes { get; set; }
    }

    public class ScimSupported
    {
        public bool Supported { get; set; }
    }

    public class ScimBulkSupported : ScimSupported
    {
        public int MaxOperations { get; set; }
        public int MaxPayloadSize { get; set; }
    }

    public class ScimFilterSupported : ScimSupported
    {
        public int MaxResults { get; set; }
    }

    public class ScimAuthenticationScheme
    {
        public string Type { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? SpecUri { get; set; }
        public string? DocumentationUri { get; set; }
    }

    public class ScimResourceTypes
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public int TotalResults { get; set; }
        public ScimResourceType[] Resources { get; set; } = Array.Empty<ScimResourceType>();
    }

    public class ScimResourceType
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Schema { get; set; } = string.Empty;
        public ScimSchemaExtension[]? SchemaExtensions { get; set; }
    }

    public class ScimSchemaExtension
    {
        public string Schema { get; set; } = string.Empty;
        public bool Required { get; set; }
    }

    public class ScimSchemas
    {
        public string[] Schemas { get; set; } = Array.Empty<string>();
        public int TotalResults { get; set; }
        public ScimSchema[] Resources { get; set; } = Array.Empty<ScimSchema>();
    }

    public class ScimSchema
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}

