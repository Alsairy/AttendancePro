using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class ActiveDirectoryService : IActiveDirectoryService
    {
        private readonly ILogger<ActiveDirectoryService> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _domain;
        private readonly string _username;
        private readonly string _password;
        private readonly string _ldapPath;

        public ActiveDirectoryService(ILogger<ActiveDirectoryService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _domain = _configuration["ActiveDirectory:Domain"];
            _username = _configuration["ActiveDirectory:Username"];
            _password = _configuration["ActiveDirectory:Password"];
            _ldapPath = _configuration["ActiveDirectory:LdapPath"];
        }

        public async Task<IEnumerable<AdUserDto>> GetUsersAsync(string searchFilter = null)
        {
            try
            {
                await Task.CompletedTask;
                var users = new List<AdUserDto>();

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    using (var searcher = new PrincipalSearcher(new UserPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            if (result is UserPrincipal user)
                            {
                                if (string.IsNullOrEmpty(searchFilter) || 
                                    user.DisplayName?.Contains(searchFilter, StringComparison.OrdinalIgnoreCase) == true ||
                                    user.UserPrincipalName?.Contains(searchFilter, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    users.Add(MapUserPrincipalToDto(user));
                                }
                            }
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get users from Active Directory");
                return new List<AdUserDto>();
            }
        }

        public async Task<AdUserDto> GetUserAsync(string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    return user != null ? MapUserPrincipalToDto(user) : null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user {UserPrincipalName} from Active Directory", userPrincipalName);
                return null;
            }
        }

        public async Task<AdUserDto> CreateUserAsync(AdUserDto userDto)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = new UserPrincipal(context)
                    {
                        UserPrincipalName = userDto.UserPrincipalName,
                        SamAccountName = userDto.SamAccountName,
                        DisplayName = userDto.DisplayName,
                        GivenName = userDto.GivenName,
                        Surname = userDto.Surname,
                        EmailAddress = userDto.Email,
                        Enabled = userDto.Enabled
                    };

                    user.Save();
                    _logger.LogInformation("Created user {UserPrincipalName} in Active Directory", userDto.UserPrincipalName);
                    return MapUserPrincipalToDto(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user {UserPrincipalName} in Active Directory", userDto.UserPrincipalName);
                return null;
            }
        }

        public async Task<AdUserDto> UpdateUserAsync(string userPrincipalName, AdUserDto userDto)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return null;

                    user.DisplayName = userDto.DisplayName;
                    user.GivenName = userDto.GivenName;
                    user.Surname = userDto.Surname;
                    user.EmailAddress = userDto.Email;
                    user.Enabled = userDto.Enabled;

                    user.Save();
                    _logger.LogInformation("Updated user {UserPrincipalName} in Active Directory", userPrincipalName);
                    return MapUserPrincipalToDto(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserPrincipalName} in Active Directory", userPrincipalName);
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return false;

                    user.Delete();
                    _logger.LogInformation("Deleted user {UserPrincipalName} from Active Directory", userPrincipalName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserPrincipalName} from Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> EnableUserAsync(string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return false;

                    user.Enabled = true;
                    user.Save();
                    _logger.LogInformation("Enabled user {UserPrincipalName} in Active Directory", userPrincipalName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enable user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> DisableUserAsync(string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return false;

                    user.Enabled = false;
                    user.Save();
                    _logger.LogInformation("Disabled user {UserPrincipalName} in Active Directory", userPrincipalName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to disable user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(string userPrincipalName, string newPassword)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return false;

                    user.SetPassword(newPassword);
                    user.ExpirePasswordNow();
                    user.Save();
                    _logger.LogInformation("Reset password for user {UserPrincipalName} in Active Directory", userPrincipalName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reset password for user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<IEnumerable<AdGroupDto>> GetGroupsAsync(string searchFilter = null)
        {
            try
            {
                await Task.CompletedTask;
                var groups = new List<AdGroupDto>();

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    using (var searcher = new PrincipalSearcher(new GroupPrincipal(context)))
                    {
                        foreach (var result in searcher.FindAll())
                        {
                            if (result is GroupPrincipal group)
                            {
                                if (string.IsNullOrEmpty(searchFilter) || 
                                    group.DisplayName?.Contains(searchFilter, StringComparison.OrdinalIgnoreCase) == true ||
                                    group.Name?.Contains(searchFilter, StringComparison.OrdinalIgnoreCase) == true)
                                {
                                    groups.Add(MapGroupPrincipalToDto(group));
                                }
                            }
                        }
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get groups from Active Directory");
                return new List<AdGroupDto>();
            }
        }

        public async Task<AdGroupDto> GetGroupAsync(string groupName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    return group != null ? MapGroupPrincipalToDto(group) : null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group {GroupName} from Active Directory", groupName);
                return null;
            }
        }

        public async Task<AdGroupDto> CreateGroupAsync(AdGroupDto groupDto)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = new GroupPrincipal(context)
                    {
                        Name = groupDto.Name,
                        DisplayName = groupDto.DisplayName,
                        Description = groupDto.Description
                    };

                    group.Save();
                    _logger.LogInformation("Created group {GroupName} in Active Directory", groupDto.Name);
                    return MapGroupPrincipalToDto(group);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create group {GroupName} in Active Directory", groupDto.Name);
                return null;
            }
        }

        public async Task<AdGroupDto> UpdateGroupAsync(string groupName, AdGroupDto groupDto)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group == null) return null;

                    group.DisplayName = groupDto.DisplayName;
                    group.Description = groupDto.Description;

                    group.Save();
                    _logger.LogInformation("Updated group {GroupName} in Active Directory", groupName);
                    return MapGroupPrincipalToDto(group);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update group {GroupName} in Active Directory", groupName);
                return null;
            }
        }

        public async Task<bool> DeleteGroupAsync(string groupName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group == null) return false;

                    group.Delete();
                    _logger.LogInformation("Deleted group {GroupName} from Active Directory", groupName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete group {GroupName} from Active Directory", groupName);
                return false;
            }
        }

        public async Task<bool> AddUserToGroupAsync(string groupName, string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                    if (group == null || user == null) return false;

                    group.Members.Add(user);
                    group.Save();
                    _logger.LogInformation("Added user {UserPrincipalName} to group {GroupName} in Active Directory", userPrincipalName, groupName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user {UserPrincipalName} to group {GroupName} in Active Directory", userPrincipalName, groupName);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromGroupAsync(string groupName, string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);

                    if (group == null || user == null) return false;

                    group.Members.Remove(user);
                    group.Save();
                    _logger.LogInformation("Removed user {UserPrincipalName} from group {GroupName} in Active Directory", userPrincipalName, groupName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user {UserPrincipalName} from group {GroupName} in Active Directory", userPrincipalName, groupName);
                return false;
            }
        }

        public async Task<IEnumerable<AdUserDto>> GetGroupMembersAsync(string groupName)
        {
            try
            {
                await Task.CompletedTask;
                var users = new List<AdUserDto>();

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var group = GroupPrincipal.FindByIdentity(context, groupName);
                    if (group == null) return users;

                    foreach (var member in group.GetMembers())
                    {
                        if (member is UserPrincipal user)
                        {
                            users.Add(MapUserPrincipalToDto(user));
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get members of group {GroupName} from Active Directory", groupName);
                return new List<AdUserDto>();
            }
        }

        public async Task<IEnumerable<AdGroupDto>> GetUserGroupsAsync(string userPrincipalName)
        {
            try
            {
                await Task.CompletedTask;
                var groups = new List<AdGroupDto>();

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return groups;

                    foreach (var group in user.GetGroups())
                    {
                        if (group is GroupPrincipal groupPrincipal)
                        {
                            groups.Add(MapGroupPrincipalToDto(groupPrincipal));
                        }
                    }
                }

                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get groups for user {UserPrincipalName} from Active Directory", userPrincipalName);
                return new List<AdGroupDto>();
            }
        }

        public async Task<IEnumerable<AdOrganizationalUnitDto>> GetOrganizationalUnitsAsync()
        {
            try
            {
                await Task.CompletedTask;
                var ous = new List<AdOrganizationalUnitDto>();

                using (var entry = new DirectoryEntry(_ldapPath, _username, _password))
                {
                    using (var searcher = new DirectorySearcher(entry))
                    {
                        searcher.Filter = "(objectClass=organizationalUnit)";
                        searcher.PropertiesToLoad.AddRange(new[] { "name", "distinguishedName", "description", "whenCreated", "whenChanged" });

                        foreach (SearchResult result in searcher.FindAll())
                        {
                            ous.Add(new AdOrganizationalUnitDto
                            {
                                Name = result.Properties["name"][0]?.ToString(),
                                DistinguishedName = result.Properties["distinguishedName"][0]?.ToString(),
                                Description = result.Properties["description"].Count > 0 ? result.Properties["description"][0]?.ToString() : null,
                                CreatedDate = result.Properties["whenCreated"].Count > 0 ? (DateTime)result.Properties["whenCreated"][0] : DateTime.MinValue,
                                ModifiedDate = result.Properties["whenChanged"].Count > 0 ? (DateTime)result.Properties["whenChanged"][0] : DateTime.MinValue
                            });
                        }
                    }
                }

                return ous;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get organizational units from Active Directory");
                return new List<AdOrganizationalUnitDto>();
            }
        }

        public async Task<bool> MoveUserToOUAsync(string userPrincipalName, string ouPath)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var user = UserPrincipal.FindByIdentity(context, userPrincipalName);
                    if (user == null) return false;

                    var userEntry = (DirectoryEntry)user.GetUnderlyingObject();
                    userEntry.MoveTo(new DirectoryEntry($"LDAP://{ouPath}", _username, _password));

                    _logger.LogInformation("Moved user {UserPrincipalName} to OU {OUPath} in Active Directory", userPrincipalName, ouPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to move user {UserPrincipalName} to OU {OUPath} in Active Directory", userPrincipalName, ouPath);
                return false;
            }
        }

        public async Task<bool> AuthenticateUserAsync(string userPrincipalName, string password)
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain))
                {
                    return context.ValidateCredentials(userPrincipalName, password);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to authenticate user {UserPrincipalName} in Active Directory", userPrincipalName);
                return false;
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    var testUser = UserPrincipal.FindByIdentity(context, _username);
                    return testUser != null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test Active Directory connection");
                return false;
            }
        }

        public async Task<AdDomainInfoDto> GetDomainInfoAsync()
        {
            try
            {
                await Task.CompletedTask;

                using (var context = new PrincipalContext(ContextType.Domain, _domain, _username, _password))
                {
                    return new AdDomainInfoDto
                    {
                        DomainName = _domain,
                        NetBiosName = _domain.Split('.')[0].ToUpper(),
                        DomainController = context.ConnectedServer,
                        ForestName = _domain,
                        IsConnected = await TestConnectionAsync(),
                        LastConnectionTest = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get domain info from Active Directory");
                return new AdDomainInfoDto
                {
                    DomainName = _domain,
                    IsConnected = false,
                    LastConnectionTest = DateTime.UtcNow
                };
            }
        }

        private AdUserDto MapUserPrincipalToDto(UserPrincipal user)
        {
            return new AdUserDto
            {
                UserPrincipalName = user.UserPrincipalName,
                SamAccountName = user.SamAccountName,
                DisplayName = user.DisplayName,
                GivenName = user.GivenName,
                Surname = user.Surname,
                Email = user.EmailAddress,
                Enabled = user.Enabled ?? false,
                DistinguishedName = user.DistinguishedName,
                LastLogon = user.LastLogon,
                PasswordLastSet = user.LastPasswordSet,
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow,
                MemberOf = new List<string>(),
                CustomAttributes = new Dictionary<string, object>()
            };
        }

        private AdGroupDto MapGroupPrincipalToDto(GroupPrincipal group)
        {
            return new AdGroupDto
            {
                Name = group.Name,
                DisplayName = group.DisplayName,
                Description = group.Description,
                DistinguishedName = group.DistinguishedName,
                GroupScope = group.GroupScope?.ToString(),
                GroupCategory = "Security",
                Members = new List<string>(),
                CreatedDate = DateTime.UtcNow,
                ModifiedDate = DateTime.UtcNow
            };
        }
    }
}
