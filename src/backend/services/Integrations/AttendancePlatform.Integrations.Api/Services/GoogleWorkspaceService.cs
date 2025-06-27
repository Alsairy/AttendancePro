using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Google.Apis.Admin.Directory.directory_v1;
using Google.Apis.Admin.Directory.directory_v1.Data;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class GoogleWorkspaceService : IGoogleWorkspaceService
    {
        private readonly ILogger<GoogleWorkspaceService> _logger;
        private readonly IConfiguration _configuration;
        private DirectoryService _directoryService;
        private CalendarService _calendarService;

        public GoogleWorkspaceService(ILogger<GoogleWorkspaceService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<GoogleUserDto>> GetUsersAsync()
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var request = directoryService.Users.List();
                request.Domain = _configuration["GoogleWorkspace:Domain"];
                request.MaxResults = 500;

                var response = await request.ExecuteAsync();
                return response.UsersValue?.Select(MapGoogleUserToDto) ?? new List<GoogleUserDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get users from Google Workspace");
                return new List<GoogleUserDto>();
            }
        }

        public async Task<GoogleUserDto> GetUserAsync(string userId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var user = await directoryService.Users.Get(userId).ExecuteAsync();
                return MapGoogleUserToDto(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user {UserId} from Google Workspace", userId);
                return null;
            }
        }

        public async Task<GoogleUserDto> CreateUserAsync(GoogleUserDto userDto)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var user = MapDtoToGoogleUser(userDto);
                var createdUser = await directoryService.Users.Insert(user).ExecuteAsync();
                return MapGoogleUserToDto(createdUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user in Google Workspace");
                return null;
            }
        }

        public async Task<GoogleUserDto> UpdateUserAsync(string userId, GoogleUserDto userDto)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var user = MapDtoToGoogleUser(userDto);
                var updatedUser = await directoryService.Users.Update(user, userId).ExecuteAsync();
                return MapGoogleUserToDto(updatedUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserId} in Google Workspace", userId);
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                await directoryService.Users.Delete(userId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserId} from Google Workspace", userId);
                return false;
            }
        }

        public async Task<IEnumerable<GoogleGroupDto>> GetGroupsAsync()
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var request = directoryService.Groups.List();
                request.Domain = _configuration["GoogleWorkspace:Domain"];
                request.MaxResults = 200;

                var response = await request.ExecuteAsync();
                return response.GroupsValue?.Select(MapGoogleGroupToDto) ?? new List<GoogleGroupDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get groups from Google Workspace");
                return new List<GoogleGroupDto>();
            }
        }

        public async Task<GoogleGroupDto> GetGroupAsync(string groupId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var group = await directoryService.Groups.Get(groupId).ExecuteAsync();
                return MapGoogleGroupToDto(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group {GroupId} from Google Workspace", groupId);
                return null;
            }
        }

        public async Task<GoogleGroupDto> CreateGroupAsync(GoogleGroupDto groupDto)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var group = MapDtoToGoogleGroup(groupDto);
                var createdGroup = await directoryService.Groups.Insert(group).ExecuteAsync();
                return MapGoogleGroupToDto(createdGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create group in Google Workspace");
                return null;
            }
        }

        public async Task<GoogleGroupDto> UpdateGroupAsync(string groupId, GoogleGroupDto groupDto)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var group = MapDtoToGoogleGroup(groupDto);
                var updatedGroup = await directoryService.Groups.Update(group, groupId).ExecuteAsync();
                return MapGoogleGroupToDto(updatedGroup);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update group {GroupId} in Google Workspace", groupId);
                return null;
            }
        }

        public async Task<bool> DeleteGroupAsync(string groupId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                await directoryService.Groups.Delete(groupId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete group {GroupId} from Google Workspace", groupId);
                return false;
            }
        }

        public async Task<bool> AddUserToGroupAsync(string groupId, string userId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var member = new Member
                {
                    Email = userId,
                    Role = "MEMBER"
                };

                await directoryService.Members.Insert(member, groupId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user {UserId} to group {GroupId} in Google Workspace", userId, groupId);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromGroupAsync(string groupId, string userId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                await directoryService.Members.Delete(groupId, userId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user {UserId} from group {GroupId} in Google Workspace", userId, groupId);
                return false;
            }
        }

        public async Task<IEnumerable<GoogleUserDto>> GetGroupMembersAsync(string groupId)
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var request = directoryService.Members.List(groupId);
                var response = await request.ExecuteAsync();

                var users = new List<GoogleUserDto>();
                if (response.MembersValue != null)
                {
                    foreach (var member in response.MembersValue)
                    {
                        if (member.Type == "USER")
                        {
                            var user = await GetUserAsync(member.Email);
                            if (user != null)
                            {
                                users.Add(user);
                            }
                        }
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get members of group {GroupId} from Google Workspace", groupId);
                return new List<GoogleUserDto>();
            }
        }

        public async Task<IEnumerable<GoogleCalendarEventDto>> GetCalendarEventsAsync(string userId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var calendarService = await GetCalendarServiceAsync();
                var request = calendarService.Events.List(userId);
                request.TimeMin = startTime;
                request.TimeMax = endTime;
                request.SingleEvents = true;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                var response = await request.ExecuteAsync();
                return response.Items?.Select(MapGoogleEventToDto) ?? new List<GoogleCalendarEventDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get calendar events for user {UserId} from Google Workspace", userId);
                return new List<GoogleCalendarEventDto>();
            }
        }

        public async Task<GoogleCalendarEventDto> CreateCalendarEventAsync(string userId, GoogleCalendarEventDto eventDto)
        {
            try
            {
                var calendarService = await GetCalendarServiceAsync();
                var calendarEvent = MapDtoToGoogleEvent(eventDto);
                var createdEvent = await calendarService.Events.Insert(calendarEvent, userId).ExecuteAsync();
                return MapGoogleEventToDto(createdEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create calendar event for user {UserId} in Google Workspace", userId);
                return null;
            }
        }

        public async Task<GoogleCalendarEventDto> UpdateCalendarEventAsync(string userId, string eventId, GoogleCalendarEventDto eventDto)
        {
            try
            {
                var calendarService = await GetCalendarServiceAsync();
                var calendarEvent = MapDtoToGoogleEvent(eventDto);
                var updatedEvent = await calendarService.Events.Update(calendarEvent, userId, eventId).ExecuteAsync();
                return MapGoogleEventToDto(updatedEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update calendar event {EventId} for user {UserId} in Google Workspace", eventId, userId);
                return null;
            }
        }

        public async Task<bool> DeleteCalendarEventAsync(string userId, string eventId)
        {
            try
            {
                var calendarService = await GetCalendarServiceAsync();
                await calendarService.Events.Delete(userId, eventId).ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete calendar event {EventId} for user {UserId} from Google Workspace", eventId, userId);
                return false;
            }
        }

        public async Task<IEnumerable<GoogleOrgUnitDto>> GetOrganizationalUnitsAsync()
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var request = directoryService.Orgunits.List(_configuration["GoogleWorkspace:CustomerId"]);
                var response = await request.ExecuteAsync();

                return response.OrganizationUnits?.Select(ou => new GoogleOrgUnitDto
                {
                    Name = ou.Name,
                    OrgUnitPath = ou.OrgUnitPath,
                    ParentOrgUnitPath = ou.ParentOrgUnitPath,
                    Description = ou.Description,
                    BlockInheritance = ou.BlockInheritance ?? false
                }) ?? new List<GoogleOrgUnitDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get organizational units from Google Workspace");
                return new List<GoogleOrgUnitDto>();
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var directoryService = await GetDirectoryServiceAsync();
                var request = directoryService.Users.List();
                request.Domain = _configuration["GoogleWorkspace:Domain"];
                request.MaxResults = 1;

                await request.ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test Google Workspace connection");
                return false;
            }
        }

        private async Task<DirectoryService> GetDirectoryServiceAsync()
        {
            if (_directoryService == null)
            {
                var serviceAccountKeyPath = _configuration["GoogleWorkspace:ServiceAccountKeyPath"];
                var adminEmail = _configuration["GoogleWorkspace:AdminEmail"];

                if (string.IsNullOrEmpty(serviceAccountKeyPath) || string.IsNullOrEmpty(adminEmail))
                {
                    throw new InvalidOperationException("Google Workspace configuration is missing");
                }

                var credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                    .CreateScoped(DirectoryService.Scope.AdminDirectoryUser, DirectoryService.Scope.AdminDirectoryGroup)
                    .CreateWithUser(adminEmail);

                _directoryService = new DirectoryService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Hudur Attendance Platform"
                });
            }

            await Task.CompletedTask;
            return _directoryService;
        }

        private async Task<CalendarService> GetCalendarServiceAsync()
        {
            if (_calendarService == null)
            {
                var serviceAccountKeyPath = _configuration["GoogleWorkspace:ServiceAccountKeyPath"];
                var adminEmail = _configuration["GoogleWorkspace:AdminEmail"];

                if (string.IsNullOrEmpty(serviceAccountKeyPath) || string.IsNullOrEmpty(adminEmail))
                {
                    throw new InvalidOperationException("Google Workspace configuration is missing");
                }

                var credential = GoogleCredential.FromFile(serviceAccountKeyPath)
                    .CreateScoped(CalendarService.Scope.Calendar)
                    .CreateWithUser(adminEmail);

                _calendarService = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Hudur Attendance Platform"
                });
            }

            await Task.CompletedTask;
            return _calendarService;
        }

        private GoogleUserDto MapGoogleUserToDto(User user)
        {
            return new GoogleUserDto
            {
                Id = user.Id,
                PrimaryEmail = user.PrimaryEmail,
                Name = new GoogleNameDto
                {
                    GivenName = user.Name?.GivenName,
                    FamilyName = user.Name?.FamilyName,
                    FullName = user.Name?.FullName
                },
                Suspended = user.Suspended ?? false,
                OrgUnitPath = user.OrgUnitPath,
                ChangePasswordAtNextLogin = user.ChangePasswordAtNextLogin ?? false,
                CreationTime = user.CreationTime ?? DateTime.MinValue,
                LastLoginTime = user.LastLoginTime ?? DateTime.MinValue
            };
        }

        private User MapDtoToGoogleUser(GoogleUserDto userDto)
        {
            return new User
            {
                PrimaryEmail = userDto.PrimaryEmail,
                Name = new UserName
                {
                    GivenName = userDto.Name?.GivenName,
                    FamilyName = userDto.Name?.FamilyName,
                    FullName = userDto.Name?.FullName
                },
                Password = userDto.Password,
                Suspended = userDto.Suspended,
                OrgUnitPath = userDto.OrgUnitPath,
                ChangePasswordAtNextLogin = userDto.ChangePasswordAtNextLogin
            };
        }

        private GoogleGroupDto MapGoogleGroupToDto(Group group)
        {
            return new GoogleGroupDto
            {
                Id = group.Id,
                Email = group.Email,
                Name = group.Name,
                Description = group.Description
            };
        }

        private Group MapDtoToGoogleGroup(GoogleGroupDto groupDto)
        {
            return new Group
            {
                Email = groupDto.Email,
                Name = groupDto.Name,
                Description = groupDto.Description
            };
        }

        private GoogleCalendarEventDto MapGoogleEventToDto(Event calendarEvent)
        {
            return new GoogleCalendarEventDto
            {
                Id = calendarEvent.Id,
                Summary = calendarEvent.Summary,
                Description = calendarEvent.Description,
                Start = new GoogleEventTimeDto
                {
                    DateTime = calendarEvent.Start?.DateTime ?? DateTime.MinValue,
                    TimeZone = calendarEvent.Start?.TimeZone,
                    Date = calendarEvent.Start?.Date
                },
                End = new GoogleEventTimeDto
                {
                    DateTime = calendarEvent.End?.DateTime ?? DateTime.MinValue,
                    TimeZone = calendarEvent.End?.TimeZone,
                    Date = calendarEvent.End?.Date
                },
                Location = calendarEvent.Location,
                Status = calendarEvent.Status,
                Visibility = calendarEvent.Visibility
            };
        }

        private Event MapDtoToGoogleEvent(GoogleCalendarEventDto eventDto)
        {
            return new Event
            {
                Summary = eventDto.Summary,
                Description = eventDto.Description,
                Start = new EventDateTime
                {
                    DateTime = eventDto.Start?.DateTime,
                    TimeZone = eventDto.Start?.TimeZone,
                    Date = eventDto.Start?.Date
                },
                End = new EventDateTime
                {
                    DateTime = eventDto.End?.DateTime,
                    TimeZone = eventDto.End?.TimeZone,
                    Date = eventDto.End?.Date
                },
                Location = eventDto.Location,
                Status = eventDto.Status,
                Visibility = eventDto.Visibility
            };
        }
    }
}
