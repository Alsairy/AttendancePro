using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.DTOs;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class MicrosoftGraphService : IMicrosoftGraphService
    {
        private readonly HudurDbContext _context;
        private readonly ILogger<MicrosoftGraphService> _logger;
        private readonly IConfiguration _configuration;

        public MicrosoftGraphService(
            HudurDbContext context,
            ILogger<MicrosoftGraphService> logger,
            IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<GraphUserDto>> SyncUsersFromAzureADAsync(Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Starting Azure AD user sync for tenant {TenantId}", tenantId);
                
                var graphClient = await GetGraphClientAsync(tenantId);
                if (graphClient == null)
                {
                    _logger.LogWarning("Could not create Graph client for tenant {TenantId}", tenantId);
                    return new List<GraphUserDto>();
                }

                var users = await graphClient.Users
                    .Request()
                    .Select("id,displayName,mail,userPrincipalName,department,jobTitle,accountEnabled")
                    .GetAsync();
                
                var result = new List<GraphUserDto>();
                foreach (var user in users)
                {
                    if (user.AccountEnabled == true)
                    {
                        result.Add(new GraphUserDto
                        {
                            Id = user.Id,
                            DisplayName = user.DisplayName,
                            Email = user.Mail ?? user.UserPrincipalName,
                            Department = user.Department,
                            JobTitle = user.JobTitle,
                            IsActive = user.AccountEnabled ?? false
                        });
                    }
                }
                
                _logger.LogInformation("Successfully synced {UserCount} users from Azure AD for tenant {TenantId}", result.Count, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing users from Azure AD for tenant {TenantId}", tenantId);
                throw new InvalidOperationException($"Azure AD user sync failed: {ex.Message}");
            }
        }

        public async Task<List<CalendarEventDto>> GetCalendarEventsAsync(Guid tenantId, Guid userId, DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("Getting calendar events for user {UserId} from {StartDate} to {EndDate}", userId, startDate, endDate);
                
                var graphClient = await GetGraphClientAsync(tenantId);
                if (graphClient == null)
                {
                    _logger.LogWarning("Could not create Graph client for tenant {TenantId}", tenantId);
                    return new List<CalendarEventDto>();
                }

                var events = await graphClient.Users[userId.ToString()].Calendar.Events
                    .Request()
                    .Filter($"start/dateTime ge '{startDate:yyyy-MM-ddTHH:mm:ss}' and end/dateTime le '{endDate:yyyy-MM-ddTHH:mm:ss}'")
                    .Select("id,subject,start,end,location,attendees,organizer")
                    .GetAsync();

                var result = events.Select(e => new CalendarEventDto
                {
                    Id = e.Id,
                    Subject = e.Subject,
                    Start = DateTime.Parse(e.Start.DateTime),
                    End = DateTime.Parse(e.End.DateTime),
                    Location = e.Location?.DisplayName,
                    Organizer = e.Organizer?.EmailAddress?.Name,
                    AttendeeCount = e.Attendees?.Count() ?? 0
                }).ToList();

                _logger.LogInformation("Retrieved {EventCount} calendar events for user {UserId}", result.Count, userId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting calendar events for user {UserId}", userId);
                throw new InvalidOperationException($"Calendar events retrieval failed: {ex.Message}");
            }
        }

        public async Task<CalendarEventDto> CreateCalendarEventAsync(Guid tenantId, CreateCalendarEventRequestDto request)
        {
            try
            {
                _logger.LogInformation("Creating calendar event for tenant {TenantId}", tenantId);
                
                var graphClient = await GetGraphClientAsync(tenantId);
                if (graphClient == null)
                {
                    throw new InvalidOperationException("Could not create Graph client");
                }

                var newEvent = new Event
                {
                    Subject = request.Subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content = request.Body
                    },
                    Start = new DateTimeTimeZone
                    {
                        DateTime = request.StartTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = "UTC"
                    },
                    End = new DateTimeTimeZone
                    {
                        DateTime = request.EndTime.ToString("yyyy-MM-ddTHH:mm:ss"),
                        TimeZone = "UTC"
                    },
                    Location = new Location
                    {
                        DisplayName = request.Location
                    }
                };

                var createdEvent = await graphClient.Users[request.UserId.ToString()].Calendar.Events
                    .Request()
                    .AddAsync(newEvent);

                var result = new CalendarEventDto
                {
                    Id = createdEvent.Id,
                    Subject = createdEvent.Subject,
                    Start = DateTime.Parse(createdEvent.Start.DateTime),
                    End = DateTime.Parse(createdEvent.End.DateTime),
                    Location = createdEvent.Location?.DisplayName,
                    Organizer = createdEvent.Organizer?.EmailAddress?.Name
                };

                _logger.LogInformation("Successfully created calendar event {EventId}", result.Id);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating calendar event for tenant {TenantId}", tenantId);
                throw new InvalidOperationException($"Calendar event creation failed: {ex.Message}");
            }
        }

        public async Task<List<TeamsChannelDto>> GetTeamsChannelsAsync(Guid tenantId)
        {
            try
            {
                _logger.LogInformation("Getting Teams channels for tenant {TenantId}", tenantId);
                
                var graphClient = await GetGraphClientAsync(tenantId);
                if (graphClient == null)
                {
                    _logger.LogWarning("Could not create Graph client for tenant {TenantId}", tenantId);
                    return new List<TeamsChannelDto>();
                }

                var teams = await graphClient.Teams
                    .Request()
                    .GetAsync();

                var result = new List<TeamsChannelDto>();
                foreach (var team in teams)
                {
                    try
                    {
                        var channels = await graphClient.Teams[team.Id].Channels
                            .Request()
                            .GetAsync();

                        foreach (var channel in channels)
                        {
                            result.Add(new TeamsChannelDto
                            {
                                Id = channel.Id,
                                Name = channel.DisplayName,
                                TeamId = team.Id,
                                TeamName = team.DisplayName,
                                Description = channel.Description
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not retrieve channels for team {TeamId}", team.Id);
                    }
                }

                _logger.LogInformation("Retrieved {ChannelCount} Teams channels for tenant {TenantId}", result.Count, tenantId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting Teams channels for tenant {TenantId}", tenantId);
                throw new InvalidOperationException($"Teams channels retrieval failed: {ex.Message}");
            }
        }

        public async Task<bool> SendTeamsMessageAsync(Guid tenantId, string channelId, string message)
        {
            try
            {
                _logger.LogInformation("Sending Teams message to channel {ChannelId}", channelId);
                
                var graphClient = await GetGraphClientAsync(tenantId);
                if (graphClient == null)
                {
                    _logger.LogWarning("Could not create Graph client for tenant {TenantId}", tenantId);
                    return false;
                }

                var chatMessage = new ChatMessage
                {
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Text,
                        Content = message
                    }
                };

                var teamId = await GetTeamIdFromChannelAsync(graphClient, channelId);
                if (string.IsNullOrEmpty(teamId))
                {
                    _logger.LogWarning("Could not resolve team ID for channel {ChannelId}", channelId);
                    return false;
                }

                await graphClient.Teams[teamId].Channels[channelId].Messages
                    .Request()
                    .AddAsync(chatMessage);

                _logger.LogInformation("Successfully sent Teams message to channel {ChannelId}", channelId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending Teams message to channel {ChannelId}", channelId);
                return false;
            }
        }

        private async Task<GraphServiceClient> GetGraphClientAsync(Guid tenantId)
        {
            try
            {
                var tenantConfig = await _context.TenantConfigurations
                    .FirstOrDefaultAsync(t => t.TenantId == tenantId);
                
                if (tenantConfig == null)
                {
                    _logger.LogWarning("No tenant configuration found for tenant {TenantId}", tenantId);
                    return null;
                }

                var clientId = tenantConfig.MicrosoftClientId ?? _configuration["MicrosoftGraph:ClientId"];
                var clientSecret = tenantConfig.MicrosoftClientSecret ?? _configuration["MicrosoftGraph:ClientSecret"];
                var tenantIdString = tenantConfig.MicrosoftTenantId ?? _configuration["MicrosoftGraph:TenantId"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(tenantIdString))
                {
                    _logger.LogWarning("Missing Microsoft Graph configuration for tenant {TenantId}", tenantId);
                    return null;
                }

                var app = ConfidentialClientApplicationBuilder
                    .Create(clientId)
                    .WithClientSecret(clientSecret)
                    .WithAuthority(new Uri($"https://login.microsoftonline.com/{tenantIdString}"))
                    .Build();

                var authProvider = new ClientCredentialProvider(app);
                return new GraphServiceClient(authProvider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Graph client for tenant {TenantId}", tenantId);
                return null;
            }
        }

        private async Task<string> GetTeamIdFromChannelAsync(GraphServiceClient graphClient, string channelId)
        {
            try
            {
                var teams = await graphClient.Teams
                    .Request()
                    .GetAsync();

                foreach (var team in teams)
                {
                    try
                    {
                        var channels = await graphClient.Teams[team.Id].Channels
                            .Request()
                            .GetAsync();

                        if (channels.Any(c => c.Id == channelId))
                        {
                            return team.Id;
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving team ID for channel {ChannelId}", channelId);
                return null;
            }
        }
    }
}
