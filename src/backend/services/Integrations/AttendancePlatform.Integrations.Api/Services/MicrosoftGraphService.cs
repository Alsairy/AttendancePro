using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Azure.Identity;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private readonly ILogger<MicrosoftGraphService> _logger;
        private readonly IConfiguration _configuration;
        private GraphServiceClient _graphServiceClient;

        public MicrosoftGraphService(ILogger<MicrosoftGraphService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                var users = await graphClient.Users.Request().GetAsync();
                return users.CurrentPage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get users from Microsoft Graph");
                return new List<User>();
            }
        }

        public async Task<User> GetUserAsync(string userId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                return await graphClient.Users[userId].Request().GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user {UserId} from Microsoft Graph", userId);
                return null;
            }
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                return await graphClient.Users.Request().AddAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user in Microsoft Graph");
                return null;
            }
        }

        public async Task<User> UpdateUserAsync(string userId, User user)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Users[userId].Request().UpdateAsync(user);
                return await GetUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update user {UserId} in Microsoft Graph", userId);
                return null;
            }
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Users[userId].Request().DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete user {UserId} from Microsoft Graph", userId);
                return false;
            }
        }

        public async Task<IEnumerable<Group>> GetGroupsAsync()
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                var groups = await graphClient.Groups.Request().GetAsync();
                return groups.CurrentPage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get groups from Microsoft Graph");
                return new List<Group>();
            }
        }

        public async Task<Group> GetGroupAsync(string groupId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                return await graphClient.Groups[groupId].Request().GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get group {GroupId} from Microsoft Graph", groupId);
                return null;
            }
        }

        public async Task<Group> CreateGroupAsync(Group group)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                return await graphClient.Groups.Request().AddAsync(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create group in Microsoft Graph");
                return null;
            }
        }

        public async Task<Group> UpdateGroupAsync(string groupId, Group group)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Groups[groupId].Request().UpdateAsync(group);
                return await GetGroupAsync(groupId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update group {GroupId} in Microsoft Graph", groupId);
                return null;
            }
        }

        public async Task<bool> DeleteGroupAsync(string groupId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Groups[groupId].Request().DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete group {GroupId} from Microsoft Graph", groupId);
                return false;
            }
        }

        public async Task<bool> AddUserToGroupAsync(string groupId, string userId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                var directoryObject = new DirectoryObject
                {
                    Id = userId
                };

                await graphClient.Groups[groupId].Members.References.Request().AddAsync(directoryObject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add user {UserId} to group {GroupId} in Microsoft Graph", userId, groupId);
                return false;
            }
        }

        public async Task<bool> RemoveUserFromGroupAsync(string groupId, string userId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Groups[groupId].Members[userId].Reference.Request().DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove user {UserId} from group {GroupId} in Microsoft Graph", userId, groupId);
                return false;
            }
        }

        public async Task<IEnumerable<User>> GetGroupMembersAsync(string groupId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                var members = await graphClient.Groups[groupId].Members.Request().GetAsync();
                var users = new List<User>();

                foreach (var member in members.CurrentPage)
                {
                    if (member is User user)
                    {
                        users.Add(user);
                    }
                }

                return users;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get members of group {GroupId} from Microsoft Graph", groupId);
                return new List<User>();
            }
        }

        public async Task<IEnumerable<Event>> GetCalendarEventsAsync(string userId, DateTime startTime, DateTime endTime)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                var events = await graphClient.Users[userId].Calendar.Events.Request()
                    .Filter($"start/dateTime ge '{startTime:yyyy-MM-ddTHH:mm:ss.fffK}' and end/dateTime le '{endTime:yyyy-MM-ddTHH:mm:ss.fffK}'")
                    .GetAsync();

                return events.CurrentPage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get calendar events for user {UserId} from Microsoft Graph", userId);
                return new List<Event>();
            }
        }

        public async Task<Event> CreateCalendarEventAsync(string userId, Event calendarEvent)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                return await graphClient.Users[userId].Calendar.Events.Request().AddAsync(calendarEvent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create calendar event for user {UserId} in Microsoft Graph", userId);
                return null;
            }
        }

        public async Task<Event> UpdateCalendarEventAsync(string userId, string eventId, Event calendarEvent)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Users[userId].Calendar.Events[eventId].Request().UpdateAsync(calendarEvent);
                return await graphClient.Users[userId].Calendar.Events[eventId].Request().GetAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update calendar event {EventId} for user {UserId} in Microsoft Graph", eventId, userId);
                return null;
            }
        }

        public async Task<bool> DeleteCalendarEventAsync(string userId, string eventId)
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Users[userId].Calendar.Events[eventId].Request().DeleteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete calendar event {EventId} for user {UserId} from Microsoft Graph", eventId, userId);
                return false;
            }
        }

        public async Task<IEnumerable<DirectoryObject>> GetDirectoryObjectsAsync()
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                var directoryObjects = await graphClient.DirectoryObjects.Request().GetAsync();
                return directoryObjects.CurrentPage;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get directory objects from Microsoft Graph");
                return new List<DirectoryObject>();
            }
        }

        public async Task<bool> TestConnectionAsync()
        {
            try
            {
                var graphClient = await GetGraphServiceClientAsync();
                await graphClient.Me.Request().GetAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to test Microsoft Graph connection");
                return false;
            }
        }

        public async Task<GraphServiceClient> GetGraphServiceClientAsync()
        {
            if (_graphServiceClient == null)
            {
                var clientId = _configuration["MicrosoftGraph:ClientId"];
                var clientSecret = _configuration["MicrosoftGraph:ClientSecret"];
                var tenantId = _configuration["MicrosoftGraph:TenantId"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantId))
                {
                    throw new InvalidOperationException("Microsoft Graph configuration is missing");
                }

                var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
                _graphServiceClient = new GraphServiceClient(credential);
            }

            await Task.CompletedTask;
            return _graphServiceClient;
        }
    }
}
