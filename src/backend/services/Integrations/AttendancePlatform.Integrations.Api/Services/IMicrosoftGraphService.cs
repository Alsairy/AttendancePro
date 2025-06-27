using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Graph;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IMicrosoftGraphService
    {
        Task<IEnumerable<User>> GetUsersAsync();
        Task<User> GetUserAsync(string userId);
        Task<User> CreateUserAsync(User user);
        Task<User> UpdateUserAsync(string userId, User user);
        Task<bool> DeleteUserAsync(string userId);
        
        Task<IEnumerable<Group>> GetGroupsAsync();
        Task<Group> GetGroupAsync(string groupId);
        Task<Group> CreateGroupAsync(Group group);
        Task<Group> UpdateGroupAsync(string groupId, Group group);
        Task<bool> DeleteGroupAsync(string groupId);
        
        Task<bool> AddUserToGroupAsync(string groupId, string userId);
        Task<bool> RemoveUserFromGroupAsync(string groupId, string userId);
        Task<IEnumerable<User>> GetGroupMembersAsync(string groupId);
        
        Task<IEnumerable<Event>> GetCalendarEventsAsync(string userId, DateTime startTime, DateTime endTime);
        Task<Event> CreateCalendarEventAsync(string userId, Event calendarEvent);
        Task<Event> UpdateCalendarEventAsync(string userId, string eventId, Event calendarEvent);
        Task<bool> DeleteCalendarEventAsync(string userId, string eventId);
        
        Task<IEnumerable<DirectoryObject>> GetDirectoryObjectsAsync();
        Task<bool> TestConnectionAsync();
        Task<GraphServiceClient> GetGraphServiceClientAsync();
    }
}
