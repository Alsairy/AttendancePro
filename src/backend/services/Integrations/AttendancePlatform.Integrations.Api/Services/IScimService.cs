using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IScimService
    {
        Task<ScimUserDto> CreateUserAsync(ScimUserDto user);
        Task<ScimUserDto> GetUserAsync(string userId);
        Task<ScimUserDto> UpdateUserAsync(string userId, ScimUserDto user);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<ScimUserDto>> GetUsersAsync(int startIndex = 1, int count = 100, string filter = null);
        
        Task<ScimGroupDto> CreateGroupAsync(ScimGroupDto group);
        Task<ScimGroupDto> GetGroupAsync(string groupId);
        Task<ScimGroupDto> UpdateGroupAsync(string groupId, ScimGroupDto group);
        Task<bool> DeleteGroupAsync(string groupId);
        Task<IEnumerable<ScimGroupDto>> GetGroupsAsync(int startIndex = 1, int count = 100, string filter = null);
        
        Task<bool> AddUserToGroupAsync(string groupId, string userId);
        Task<bool> RemoveUserFromGroupAsync(string groupId, string userId);
        
        Task<ScimResourceTypeDto> GetResourceTypeAsync(string resourceType);
        Task<IEnumerable<ScimResourceTypeDto>> GetResourceTypesAsync();
        Task<ScimSchemaDto> GetSchemaAsync(string schemaId);
        Task<IEnumerable<ScimSchemaDto>> GetSchemasAsync();
        Task<ScimServiceProviderConfigDto> GetServiceProviderConfigAsync();
    }

    public class ScimUserDto
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string UserName { get; set; }
        public ScimNameDto Name { get; set; }
        public string DisplayName { get; set; }
        public string NickName { get; set; }
        public string ProfileUrl { get; set; }
        public string Title { get; set; }
        public string UserType { get; set; }
        public string PreferredLanguage { get; set; }
        public string Locale { get; set; }
        public string Timezone { get; set; }
        public bool Active { get; set; }
        public string Password { get; set; }
        public List<ScimEmailDto> Emails { get; set; }
        public List<ScimPhoneNumberDto> PhoneNumbers { get; set; }
        public List<ScimAddressDto> Addresses { get; set; }
        public List<ScimGroupMembershipDto> Groups { get; set; }
        public ScimEnterpriseUserDto EnterpriseUser { get; set; }
        public ScimMetaDto Meta { get; set; }
    }

    public class ScimGroupDto
    {
        public string Id { get; set; }
        public string ExternalId { get; set; }
        public string DisplayName { get; set; }
        public List<ScimGroupMemberDto> Members { get; set; }
        public ScimMetaDto Meta { get; set; }
    }

    public class ScimNameDto
    {
        public string Formatted { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string MiddleName { get; set; }
        public string HonorificPrefix { get; set; }
        public string HonorificSuffix { get; set; }
    }

    public class ScimEmailDto
    {
        public string Value { get; set; }
        public string Display { get; set; }
        public string Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimPhoneNumberDto
    {
        public string Value { get; set; }
        public string Display { get; set; }
        public string Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimAddressDto
    {
        public string Formatted { get; set; }
        public string StreetAddress { get; set; }
        public string Locality { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Type { get; set; }
        public bool Primary { get; set; }
    }

    public class ScimGroupMembershipDto
    {
        public string Value { get; set; }
        public string Ref { get; set; }
        public string Display { get; set; }
        public string Type { get; set; }
    }

    public class ScimGroupMemberDto
    {
        public string Value { get; set; }
        public string Ref { get; set; }
        public string Display { get; set; }
        public string Type { get; set; }
    }

    public class ScimEnterpriseUserDto
    {
        public string EmployeeNumber { get; set; }
        public string CostCenter { get; set; }
        public string Organization { get; set; }
        public string Division { get; set; }
        public string Department { get; set; }
        public ScimManagerDto Manager { get; set; }
    }

    public class ScimManagerDto
    {
        public string Value { get; set; }
        public string Ref { get; set; }
        public string DisplayName { get; set; }
    }

    public class ScimMetaDto
    {
        public string ResourceType { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }
        public string Location { get; set; }
        public string Version { get; set; }
    }

    public class ScimResourceTypeDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Endpoint { get; set; }
        public string Schema { get; set; }
        public List<string> SchemaExtensions { get; set; }
        public ScimMetaDto Meta { get; set; }
    }

    public class ScimSchemaDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ScimAttributeDto> Attributes { get; set; }
        public ScimMetaDto Meta { get; set; }
    }

    public class ScimAttributeDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public bool MultiValued { get; set; }
        public bool Required { get; set; }
        public string Mutability { get; set; }
        public string Returned { get; set; }
        public string Uniqueness { get; set; }
        public List<string> CanonicalValues { get; set; }
        public List<ScimAttributeDto> SubAttributes { get; set; }
    }

    public class ScimServiceProviderConfigDto
    {
        public ScimBulkDto Bulk { get; set; }
        public ScimFilterDto Filter { get; set; }
        public ScimChangePasswordDto ChangePassword { get; set; }
        public ScimSortDto Sort { get; set; }
        public ScimPatchDto Patch { get; set; }
        public ScimETagDto ETag { get; set; }
        public List<ScimAuthenticationSchemeDto> AuthenticationSchemes { get; set; }
        public ScimMetaDto Meta { get; set; }
    }

    public class ScimBulkDto
    {
        public bool Supported { get; set; }
        public int MaxOperations { get; set; }
        public int MaxPayloadSize { get; set; }
    }

    public class ScimFilterDto
    {
        public bool Supported { get; set; }
        public int MaxResults { get; set; }
    }

    public class ScimChangePasswordDto
    {
        public bool Supported { get; set; }
    }

    public class ScimSortDto
    {
        public bool Supported { get; set; }
    }

    public class ScimPatchDto
    {
        public bool Supported { get; set; }
    }

    public class ScimETagDto
    {
        public bool Supported { get; set; }
    }

    public class ScimAuthenticationSchemeDto
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SpecUri { get; set; }
        public string DocumentationUri { get; set; }
    }
}
