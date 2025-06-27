using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IActiveDirectoryService
    {
        Task<IEnumerable<AdUserDto>> GetUsersAsync(string searchFilter = null);
        Task<AdUserDto> GetUserAsync(string userPrincipalName);
        Task<AdUserDto> CreateUserAsync(AdUserDto user);
        Task<AdUserDto> UpdateUserAsync(string userPrincipalName, AdUserDto user);
        Task<bool> DeleteUserAsync(string userPrincipalName);
        Task<bool> EnableUserAsync(string userPrincipalName);
        Task<bool> DisableUserAsync(string userPrincipalName);
        Task<bool> ResetPasswordAsync(string userPrincipalName, string newPassword);
        
        Task<IEnumerable<AdGroupDto>> GetGroupsAsync(string searchFilter = null);
        Task<AdGroupDto> GetGroupAsync(string groupName);
        Task<AdGroupDto> CreateGroupAsync(AdGroupDto group);
        Task<AdGroupDto> UpdateGroupAsync(string groupName, AdGroupDto group);
        Task<bool> DeleteGroupAsync(string groupName);
        
        Task<bool> AddUserToGroupAsync(string groupName, string userPrincipalName);
        Task<bool> RemoveUserFromGroupAsync(string groupName, string userPrincipalName);
        Task<IEnumerable<AdUserDto>> GetGroupMembersAsync(string groupName);
        Task<IEnumerable<AdGroupDto>> GetUserGroupsAsync(string userPrincipalName);
        
        Task<IEnumerable<AdOrganizationalUnitDto>> GetOrganizationalUnitsAsync();
        Task<bool> MoveUserToOUAsync(string userPrincipalName, string ouPath);
        
        Task<bool> AuthenticateUserAsync(string userPrincipalName, string password);
        Task<bool> TestConnectionAsync();
        Task<AdDomainInfoDto> GetDomainInfoAsync();
    }

    public class AdUserDto
    {
        public string UserPrincipalName { get; set; }
        public string SamAccountName { get; set; }
        public string DisplayName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string Title { get; set; }
        public string Manager { get; set; }
        public string OfficePhone { get; set; }
        public string MobilePhone { get; set; }
        public string Office { get; set; }
        public string Company { get; set; }
        public string DistinguishedName { get; set; }
        public bool Enabled { get; set; }
        public bool PasswordNeverExpires { get; set; }
        public bool MustChangePasswordAtNextLogon { get; set; }
        public DateTime? LastLogon { get; set; }
        public DateTime? PasswordLastSet { get; set; }
        public DateTime? AccountExpirationDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public List<string> MemberOf { get; set; }
        public Dictionary<string, object> CustomAttributes { get; set; }
    }

    public class AdGroupDto
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public string DistinguishedName { get; set; }
        public string GroupScope { get; set; }
        public string GroupCategory { get; set; }
        public List<string> Members { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class AdOrganizationalUnitDto
    {
        public string Name { get; set; }
        public string DistinguishedName { get; set; }
        public string Description { get; set; }
        public string ParentOU { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }

    public class AdDomainInfoDto
    {
        public string DomainName { get; set; }
        public string NetBiosName { get; set; }
        public string DomainController { get; set; }
        public string ForestName { get; set; }
        public int DomainFunctionalLevel { get; set; }
        public int ForestFunctionalLevel { get; set; }
        public bool IsConnected { get; set; }
        public DateTime LastConnectionTest { get; set; }
    }
}
