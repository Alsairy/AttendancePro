namespace AttendancePlatform.Application.Services;

public interface IActiveDirectoryService
{
    Task<IEnumerable<AdUserDto>> GetUsersAsync(string searchFilter = null);
    Task<AdUserDto> GetUserAsync(string userPrincipalName);
    Task<AdUserDto> CreateUserAsync(AdUserDto userDto);
    Task<AdUserDto> UpdateUserAsync(string userPrincipalName, AdUserDto userDto);
    Task<bool> DeleteUserAsync(string userPrincipalName);
    Task<bool> EnableUserAsync(string userPrincipalName);
    Task<bool> DisableUserAsync(string userPrincipalName);
    Task<bool> ResetPasswordAsync(string userPrincipalName, string newPassword);
    Task<IEnumerable<AdGroupDto>> GetGroupsAsync(string searchFilter = null);
    Task<AdGroupDto> GetGroupAsync(string groupName);
    Task<AdGroupDto> CreateGroupAsync(AdGroupDto groupDto);
    Task<AdGroupDto> UpdateGroupAsync(string groupName, AdGroupDto groupDto);
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
    public string UserPrincipalName { get; set; } = string.Empty;
    public string SamAccountName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool Enabled { get; set; }
    public DateTime? LastLogon { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class AdGroupDto
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

public class AdOrganizationalUnitDto
{
    public string Name { get; set; } = string.Empty;
    public string DistinguishedName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

public class AdDomainInfoDto
{
    public string DomainName { get; set; } = string.Empty;
    public string NetBiosName { get; set; } = string.Empty;
    public string DomainController { get; set; } = string.Empty;
    public string ForestName { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime LastConnectionTest { get; set; }
}
