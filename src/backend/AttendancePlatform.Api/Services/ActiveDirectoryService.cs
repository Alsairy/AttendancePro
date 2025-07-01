using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using AttendancePlatform.Application.Services;
using Microsoft.Extensions.Configuration;

namespace AttendancePlatform.Api.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private readonly string _ldapPath;

        public ActiveDirectoryService(IConfiguration configuration, ILogger<ActiveDirectoryService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _domain = _configuration["ActiveDirectory:Domain"] ?? "localhost";
            _username = _configuration["ActiveDirectory:Username"] ?? "admin";
            _password = _configuration["ActiveDirectory:Password"] ?? "password";
            _ldapPath = _configuration["ActiveDirectory:LdapPath"] ?? "LDAP://localhost";
        }

        public async Task<IEnumerable<AdUserDto>> GetUsersAsync(string searchFilter = null)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Returning empty list for users");
                    return new List<AdUserDto>();
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var searcher = new UserPrincipal(context);
                
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    searcher.Name = $"*{searchFilter}*";
                }

                using var searchResults = new PrincipalSearcher(searcher);
                var users = new List<AdUserDto>();

                foreach (UserPrincipal user in searchResults.FindAll())
                {
                    if (user != null)
                    {
                        users.Add(new AdUserDto
                        {
                            UserPrincipalName = user.UserPrincipalName ?? string.Empty,
                            SamAccountName = user.SamAccountName ?? string.Empty,
                            DisplayName = user.DisplayName ?? string.Empty,
                            GivenName = user.GivenName ?? string.Empty,
                            Surname = user.Surname ?? string.Empty,
                            Email = user.EmailAddress ?? string.Empty,
                            Enabled = user.Enabled ?? false,
                            LastLogon = user.LastLogon,
                            CreatedDate = DateTime.UtcNow,
                            ModifiedDate = user.LastPasswordSet
                        });
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting users from Active Directory");
                return new List<AdUserDto>();
            }
        }

        public async Task<AdUserDto> GetUserAsync(string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot get user {UserPrincipalName}", userPrincipalName);
                    throw new PlatformNotSupportedException("Active Directory operations are only supported on Windows");
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    throw new InvalidOperationException($"User {userPrincipalName} not found");
                }

                return new AdUserDto
                {
                    UserPrincipalName = user.UserPrincipalName ?? string.Empty,
                    SamAccountName = user.SamAccountName ?? string.Empty,
                    DisplayName = user.DisplayName ?? string.Empty,
                    GivenName = user.GivenName ?? string.Empty,
                    Surname = user.Surname ?? string.Empty,
                    Email = user.EmailAddress ?? string.Empty,
                    Enabled = user.Enabled ?? false,
                    LastLogon = user.LastLogon,
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = user.LastPasswordSet
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user {UserPrincipalName} from Active Directory", userPrincipalName);
                throw;
            }
        }

        public async Task<AdUserDto> CreateUserAsync(AdUserDto userDto)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot create user {UserPrincipalName}", userDto.UserPrincipalName);
                    throw new PlatformNotSupportedException("Active Directory operations are only supported on Windows");
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = new UserPrincipal(context)
                {
                    UserPrincipalName = userDto.UserPrincipalName,
                    SamAccountName = userDto.SamAccountName,
                    DisplayName = userDto.DisplayName,
                    GivenName = userDto.GivenName,
                    Surname = userDto.Surname,
                    EmailAddress = userDto.Email,
                    Enabled = userDto.Enabled
                };

                if (OperatingSystem.IsWindows())
                {
                    user.Save();
                }
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {UserPrincipalName} in Active Directory", userDto.UserPrincipalName);
                throw;
            }
        }

        public async Task<AdUserDto> UpdateUserAsync(string userPrincipalName, AdUserDto userDto)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot update user {UserPrincipalName}", userPrincipalName);
                    throw new PlatformNotSupportedException("Active Directory operations are only supported on Windows");
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    throw new InvalidOperationException($"User {userPrincipalName} not found");
                }

                user.DisplayName = userDto.DisplayName;
                user.GivenName = userDto.GivenName;
                user.Surname = userDto.Surname;
                user.EmailAddress = userDto.Email;
                user.Enabled = userDto.Enabled;

                if (OperatingSystem.IsWindows())
                {
                    user.Save();
                }
                return userDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {UserPrincipalName} in Active Directory", userPrincipalName);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot delete user {UserPrincipalName}", userPrincipalName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    return false;
                }

                if (OperatingSystem.IsWindows())
                {
                    user.Delete();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserPrincipalName} from Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> EnableUserAsync(string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot enable user {UserPrincipalName}", userPrincipalName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    return false;
                }

                user.Enabled = true;
                if (OperatingSystem.IsWindows())
                {
                    user.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> DisableUserAsync(string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot disable user {UserPrincipalName}", userPrincipalName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    return false;
                }

                user.Enabled = false;
                if (OperatingSystem.IsWindows())
                {
                    user.Save();
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string userPrincipalName, string newPassword)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot reset password for user {UserPrincipalName}", userPrincipalName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    return false;
                }

                user.SetPassword(newPassword);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password for user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<IEnumerable<AdGroupDto>> GetGroupsAsync(string searchFilter = null)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Returning empty list for groups");
                    return new List<AdGroupDto>();
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var searcher = new GroupPrincipal(context);
                
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    searcher.Name = $"*{searchFilter}*";
                }

                using var searchResults = new PrincipalSearcher(searcher);
                var groups = new List<AdGroupDto>();

                foreach (GroupPrincipal group in searchResults.FindAll())
                {
                    if (group != null)
                    {
                        groups.Add(new AdGroupDto
                        {
                            Name = group.Name ?? string.Empty,
                            DisplayName = group.DisplayName ?? string.Empty,
                            Description = group.Description ?? string.Empty
                        });
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups from Active Directory");
                return new List<AdGroupDto>();
            }
        }

        public async Task<AdGroupDto> GetGroupAsync(string groupName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot get group {GroupName}", groupName);
                    throw new PlatformNotSupportedException("Active Directory operations are only supported on Windows");
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = GroupPrincipal.FindByIdentity(context, groupName);

                if (group == null)
                {
                    throw new InvalidOperationException($"Group {groupName} not found");
                }

                return new AdGroupDto
                {
                    Name = group.Name ?? string.Empty,
                    DisplayName = group.DisplayName ?? string.Empty,
                    Description = group.Description ?? string.Empty
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting group {GroupName} from Active Directory", groupName);
                throw;
            }
        }

        public async Task<AdGroupDto> CreateGroupAsync(AdGroupDto groupDto)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot create group {GroupName}", groupDto.Name);
                    throw new PlatformNotSupportedException("Active Directory operations are only supported on Windows");
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = new GroupPrincipal(context)
                {
                    Name = groupDto.Name,
                    DisplayName = groupDto.DisplayName,
                    Description = groupDto.Description
                };

                group.Save();
                return groupDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating group {GroupName} in Active Directory", groupDto.Name);
                throw;
            }
        }

        public async Task<AdGroupDto> UpdateGroupAsync(string groupName, AdGroupDto groupDto)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot update group {GroupName}", groupName);
                    throw new PlatformNotSupportedException("Active Directory operations are only supported on Windows");
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = GroupPrincipal.FindByIdentity(context, groupName);

                if (group == null)
                {
                    throw new InvalidOperationException($"Group {groupName} not found");
                }

                group.DisplayName = groupDto.DisplayName;
                group.Description = groupDto.Description;

                group.Save();
                return groupDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating group {GroupName} in Active Directory", groupName);
                throw;
            }
        }

        public async Task<bool> DeleteGroupAsync(string groupName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot delete group {GroupName}", groupName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = GroupPrincipal.FindByIdentity(context, groupName);

                if (group == null)
                {
                    return false;
                }

                group.Delete();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting group {GroupName} from Active Directory", groupName);
                return false;
            }
        }

        public async Task<bool> AddUserToGroupAsync(string groupName, string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot add user {UserPrincipalName} to group {GroupName}", userPrincipalName, groupName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = GroupPrincipal.FindByIdentity(context, groupName);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (group == null || user == null)
                {
                    return false;
                }

                group.Members.Add(user);
                group.Save();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding user {UserPrincipalName} to group {GroupName}", userPrincipalName, groupName);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromGroupAsync(string groupName, string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot remove user {UserPrincipalName} from group {GroupName}", userPrincipalName, groupName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = GroupPrincipal.FindByIdentity(context, groupName);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (group == null || user == null)
                {
                    return false;
                }

                group.Members.Remove(user);
                group.Save();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing user {UserPrincipalName} from group {GroupName}", userPrincipalName, groupName);
                return false;
            }
        }

        public async Task<IEnumerable<AdUserDto>> GetGroupMembersAsync(string groupName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Returning empty list for group {GroupName}", groupName);
                    return new List<AdUserDto>();
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var group = GroupPrincipal.FindByIdentity(context, groupName);

                if (group == null)
                {
                    return new List<AdUserDto>();
                }

                var members = new List<AdUserDto>();
                foreach (Principal member in group.GetMembers())
                {
                    if (member is UserPrincipal user)
                    {
                        members.Add(new AdUserDto
                        {
                            UserPrincipalName = user.UserPrincipalName ?? string.Empty,
                            SamAccountName = user.SamAccountName ?? string.Empty,
                            DisplayName = user.DisplayName ?? string.Empty,
                            GivenName = user.GivenName ?? string.Empty,
                            Surname = user.Surname ?? string.Empty,
                            Email = user.EmailAddress ?? string.Empty,
                            Enabled = user.Enabled ?? false
                        });
                    }
                }

                return members;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting members of group {GroupName}", groupName);
                return new List<AdUserDto>();
            }
        }

        public async Task<IEnumerable<AdGroupDto>> GetUserGroupsAsync(string userPrincipalName)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Returning empty list for user {UserPrincipalName}", userPrincipalName);
                    return new List<AdGroupDto>();
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    return new List<AdGroupDto>();
                }

                var groups = new List<AdGroupDto>();
                foreach (Principal group in user.GetGroups())
                {
                    if (group is GroupPrincipal groupPrincipal)
                    {
                        groups.Add(new AdGroupDto
                        {
                            Name = groupPrincipal.Name ?? string.Empty,
                            DisplayName = groupPrincipal.DisplayName ?? string.Empty,
                            Description = groupPrincipal.Description ?? string.Empty
                        });
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting groups for user {UserPrincipalName}", userPrincipalName);
                return new List<AdGroupDto>();
            }
        }

        public async Task<IEnumerable<AdOrganizationalUnitDto>> GetOrganizationalUnitsAsync()
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Returning empty list for organizational units");
                    return new List<AdOrganizationalUnitDto>();
                }

                var ous = new List<AdOrganizationalUnitDto>();
                using var entry = new DirectoryEntry(_ldapPath, _username, _password);
                using var searcher = new DirectorySearcher(entry)
                {
                    Filter = "(objectClass=organizationalUnit)"
                };

                var results = searcher.FindAll();
                foreach (SearchResult result in results)
                {
                    var ou = new AdOrganizationalUnitDto
                    {
                        Name = result.Properties["name"][0]?.ToString() ?? string.Empty,
                        DistinguishedName = result.Properties["distinguishedName"][0]?.ToString() ?? string.Empty,
                        Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0]?.ToString() ?? string.Empty : string.Empty,
                        CreatedDate = DateTime.Now,
                        ModifiedDate = DateTime.Now
                    };
                    ous.Add(ou);
                }

                return ous;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting organizational units from Active Directory");
                return new List<AdOrganizationalUnitDto>();
            }
        }

        public async Task<bool> MoveUserToOUAsync(string userPrincipalName, string ouPath)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot move user {UserPrincipalName} to OU {OUPath}", userPrincipalName, ouPath);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                if (user == null)
                {
                    return false;
                }

                var directoryEntry = (DirectoryEntry)user.GetUnderlyingObject();
                directoryEntry.MoveTo(new DirectoryEntry($"LDAP://{ouPath}", _username, _password));
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving user {UserPrincipalName} to OU {OUPath}", userPrincipalName, ouPath);
                return false;
            }
        }

        public async Task<bool> AuthenticateUserAsync(string userPrincipalName, string password)
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Cannot authenticate user {UserPrincipalName}", userPrincipalName);
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain);
                return context.ValidateCredentials(userPrincipalName, password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user {UserPrincipalName}", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Connection test failed");
                    return false;
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                using var searcher = new UserPrincipal(context);
                using var searchResults = new PrincipalSearcher(searcher);
                
                var firstUser = searchResults.FindOne();
                return firstUser != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Active Directory connection");
                return false;
            }
        }

        public async Task<AdDomainInfoDto> GetDomainInfoAsync()
        {
            try
            {
                if (!OperatingSystem.IsWindows())
                {
                    _logger.LogWarning("Active Directory operations are only supported on Windows. Returning disconnected domain info");
                    return new AdDomainInfoDto
                    {
                        DomainName = _domain,
                        NetBiosName = _domain.Split('.')[0],
                        DomainController = _domain,
                        ForestName = _domain,
                        IsConnected = false,
                        LastConnectionTest = DateTime.UtcNow
                    };
                }

                using var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password);
                
                return new AdDomainInfoDto
                {
                    DomainName = _domain,
                    NetBiosName = _domain.Split('.')[0],
                    DomainController = _domain,
                    ForestName = _domain,
                    IsConnected = await TestConnectionAsync(),
                    LastConnectionTest = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting domain information");
                return new AdDomainInfoDto
                {
                    DomainName = _domain,
                    IsConnected = false,
                    LastConnectionTest = DateTime.UtcNow
                };
            }
        }
    }
}
