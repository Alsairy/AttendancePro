using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AttendancePlatform.Integrations.Api.Services
{
    public interface IGoogleWorkspaceService
    {
        Task<IEnumerable<GoogleUserDto>> GetUsersAsync();
        Task<GoogleUserDto> GetUserAsync(string userId);
        Task<GoogleUserDto> CreateUserAsync(GoogleUserDto user);
        Task<GoogleUserDto> UpdateUserAsync(string userId, GoogleUserDto user);
        Task<bool> DeleteUserAsync(string userId);
        
        Task<IEnumerable<GoogleGroupDto>> GetGroupsAsync();
        Task<GoogleGroupDto> GetGroupAsync(string groupId);
        Task<GoogleGroupDto> CreateGroupAsync(GoogleGroupDto group);
        Task<GoogleGroupDto> UpdateGroupAsync(string groupId, GoogleGroupDto group);
        Task<bool> DeleteGroupAsync(string groupId);
        
        Task<bool> AddUserToGroupAsync(string groupId, string userId);
        Task<bool> RemoveUserFromGroupAsync(string groupId, string userId);
        Task<IEnumerable<GoogleUserDto>> GetGroupMembersAsync(string groupId);
        
        Task<IEnumerable<GoogleCalendarEventDto>> GetCalendarEventsAsync(string userId, DateTime startTime, DateTime endTime);
        Task<GoogleCalendarEventDto> CreateCalendarEventAsync(string userId, GoogleCalendarEventDto calendarEvent);
        Task<GoogleCalendarEventDto> UpdateCalendarEventAsync(string userId, string eventId, GoogleCalendarEventDto calendarEvent);
        Task<bool> DeleteCalendarEventAsync(string userId, string eventId);
        
        Task<IEnumerable<GoogleOrgUnitDto>> GetOrganizationalUnitsAsync();
        Task<bool> TestConnectionAsync();
    }

    public class GoogleUserDto
    {
        public string Id { get; set; }
        public string PrimaryEmail { get; set; }
        public GoogleNameDto Name { get; set; }
        public string Password { get; set; }
        public bool Suspended { get; set; }
        public string OrgUnitPath { get; set; }
        public bool ChangePasswordAtNextLogin { get; set; }
        public List<GoogleEmailDto> Emails { get; set; }
        public List<GooglePhoneDto> Phones { get; set; }
        public GoogleCustomSchemaDto CustomSchemas { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastLoginTime { get; set; }
    }

    public class GoogleGroupDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GoogleGroupMemberDto> Members { get; set; }
        public GoogleGroupSettingsDto Settings { get; set; }
    }

    public class GoogleNameDto
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullName { get; set; }
    }

    public class GoogleEmailDto
    {
        public string Address { get; set; }
        public string Type { get; set; }
        public bool Primary { get; set; }
    }

    public class GooglePhoneDto
    {
        public string Value { get; set; }
        public string Type { get; set; }
        public bool Primary { get; set; }
    }

    public class GoogleCustomSchemaDto
    {
        public Dictionary<string, object> EmployeeInfo { get; set; }
    }

    public class GoogleGroupMemberDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
    }

    public class GoogleGroupSettingsDto
    {
        public string WhoCanJoin { get; set; }
        public string WhoCanViewMembership { get; set; }
        public string WhoCanViewGroup { get; set; }
        public string WhoCanInvite { get; set; }
        public bool AllowExternalMembers { get; set; }
        public bool WhoCanPostMessage { get; set; }
    }

    public class GoogleCalendarEventDto
    {
        public string Id { get; set; }
        public string Summary { get; set; }
        public string Description { get; set; }
        public GoogleEventTimeDto Start { get; set; }
        public GoogleEventTimeDto End { get; set; }
        public List<GoogleAttendeeDto> Attendees { get; set; }
        public string Location { get; set; }
        public string Status { get; set; }
        public string Visibility { get; set; }
        public List<string> Recurrence { get; set; }
    }

    public class GoogleEventTimeDto
    {
        public DateTime DateTime { get; set; }
        public string TimeZone { get; set; }
        public string Date { get; set; }
    }

    public class GoogleAttendeeDto
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string ResponseStatus { get; set; }
        public bool Optional { get; set; }
    }

    public class GoogleOrgUnitDto
    {
        public string Name { get; set; }
        public string OrgUnitPath { get; set; }
        public string ParentOrgUnitPath { get; set; }
        public string Description { get; set; }
        public bool BlockInheritance { get; set; }
    }
}
