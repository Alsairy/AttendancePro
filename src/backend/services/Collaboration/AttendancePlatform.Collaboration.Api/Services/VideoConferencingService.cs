using Microsoft.EntityFrameworkCore;
using Hudur.Shared.Infrastructure.Data;
using Hudur.Shared.Domain.Entities;

namespace Hudur.Collaboration.Api.Services
{
    public interface IVideoConferencingService
    {
        Task<VideoConference> CreateConferenceAsync(string title, Guid organizerId, DateTime scheduledTime, int maxParticipants = 50);
        Task<VideoConference> JoinConferenceAsync(Guid conferenceId, Guid userId);
        Task<bool> LeaveConferenceAsync(Guid conferenceId, Guid userId);
        Task<VideoConference> StartConferenceAsync(Guid conferenceId, Guid organizerId);
        Task<bool> EndConferenceAsync(Guid conferenceId, Guid organizerId);
        Task<IEnumerable<VideoConference>> GetUserConferencesAsync(Guid userId);
        Task<VideoConference?> GetActiveConferenceAsync(Guid conferenceId);
        Task<bool> MuteParticipantAsync(Guid conferenceId, Guid participantId, Guid moderatorId);
        Task<bool> KickParticipantAsync(Guid conferenceId, Guid participantId, Guid moderatorId);
        Task<string> GenerateConferenceTokenAsync(Guid conferenceId, Guid userId);
    }

    public class VideoConferencingService : IVideoConferencingService
    {
        private readonly HudurDbContext _context;
        private readonly ILogger<VideoConferencingService> _logger;

        public VideoConferencingService(HudurDbContext context, ILogger<VideoConferencingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<VideoConference> CreateConferenceAsync(string title, Guid organizerId, DateTime scheduledTime, int maxParticipants = 50)
        {
            try
            {
                var conference = new VideoConference
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    OrganizerId = organizerId,
                    ScheduledTime = scheduledTime,
                    MaxParticipants = maxParticipants,
                    Status = ConferenceStatus.Scheduled,
                    CreatedAt = DateTime.UtcNow,
                    ConferenceUrl = $"https://meet.hudu.sa/{Guid.NewGuid():N}",
                    IsRecordingEnabled = false,
                    IsLocked = false
                };

                _context.VideoConferences.Add(conference);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Video conference created successfully. ConferenceId: {ConferenceId}, Title: {Title}", 
                    conference.Id, title);

                return conference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating video conference. Title: {Title}, OrganizerId: {OrganizerId}", 
                    title, organizerId);
                throw;
            }
        }

        public async Task<VideoConference> JoinConferenceAsync(Guid conferenceId, Guid userId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == conferenceId);

                if (conference == null)
                    throw new ArgumentException("Conference not found");

                if (conference.IsLocked)
                    throw new UnauthorizedAccessException("Conference is locked");

                if (conference.Participants.Count >= conference.MaxParticipants)
                    throw new InvalidOperationException("Conference is full");

                var existingParticipant = conference.Participants
                    .FirstOrDefault(p => p.UserId == userId);

                if (existingParticipant == null)
                {
                    var participant = new ConferenceParticipant
                    {
                        Id = Guid.NewGuid(),
                        ConferenceId = conferenceId,
                        UserId = userId,
                        JoinedAt = DateTime.UtcNow,
                        IsMuted = false,
                        IsVideoEnabled = true,
                        Role = userId == conference.OrganizerId ? "Moderator" : "Participant"
                    };

                    _context.ConferenceParticipants.Add(participant);
                    await _context.SaveChangesAsync();
                }

                _logger.LogInformation("User joined conference successfully. ConferenceId: {ConferenceId}, UserId: {UserId}", 
                    conferenceId, userId);

                return conference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining conference. ConferenceId: {ConferenceId}, UserId: {UserId}", 
                    conferenceId, userId);
                throw;
            }
        }

        public async Task<bool> LeaveConferenceAsync(Guid conferenceId, Guid userId)
        {
            try
            {
                var participant = await _context.ConferenceParticipants
                    .FirstOrDefaultAsync(p => p.ConferenceId == conferenceId && p.UserId == userId);

                if (participant == null)
                    return false;

                participant.LeftAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User left conference successfully. ConferenceId: {ConferenceId}, UserId: {UserId}", 
                    conferenceId, userId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving conference. ConferenceId: {ConferenceId}, UserId: {UserId}", 
                    conferenceId, userId);
                throw;
            }
        }

        public async Task<VideoConference> StartConferenceAsync(Guid conferenceId, Guid organizerId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .FirstOrDefaultAsync(c => c.Id == conferenceId && c.OrganizerId == organizerId);

                if (conference == null)
                    throw new UnauthorizedAccessException("Conference not found or user not authorized");

                conference.Status = ConferenceStatus.InProgress;
                conference.StartedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Conference started successfully. ConferenceId: {ConferenceId}, OrganizerId: {OrganizerId}", 
                    conferenceId, organizerId);

                return conference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting conference. ConferenceId: {ConferenceId}, OrganizerId: {OrganizerId}", 
                    conferenceId, organizerId);
                throw;
            }
        }

        public async Task<bool> EndConferenceAsync(Guid conferenceId, Guid organizerId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .FirstOrDefaultAsync(c => c.Id == conferenceId && c.OrganizerId == organizerId);

                if (conference == null)
                    return false;

                conference.Status = ConferenceStatus.Ended;
                conference.EndedAt = DateTime.UtcNow;

                var activeParticipants = await _context.ConferenceParticipants
                    .Where(p => p.ConferenceId == conferenceId && p.LeftAt == null)
                    .ToListAsync();

                foreach (var participant in activeParticipants)
                {
                    participant.LeftAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Conference ended successfully. ConferenceId: {ConferenceId}, OrganizerId: {OrganizerId}", 
                    conferenceId, organizerId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error ending conference. ConferenceId: {ConferenceId}, OrganizerId: {OrganizerId}", 
                    conferenceId, organizerId);
                throw;
            }
        }

        public async Task<IEnumerable<VideoConference>> GetUserConferencesAsync(Guid userId)
        {
            try
            {
                var conferences = await _context.VideoConferences
                    .Include(c => c.Participants)
                    .Where(c => c.OrganizerId == userId || c.Participants.Any(p => p.UserId == userId))
                    .OrderByDescending(c => c.ScheduledTime)
                    .ToListAsync();

                return conferences;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user conferences. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<VideoConference?> GetActiveConferenceAsync(Guid conferenceId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .Include(c => c.Participants.Where(p => p.LeftAt == null))
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(c => c.Id == conferenceId && c.Status == ConferenceStatus.InProgress);

                return conference;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active conference. ConferenceId: {ConferenceId}", conferenceId);
                throw;
            }
        }

        public async Task<bool> MuteParticipantAsync(Guid conferenceId, Guid participantId, Guid moderatorId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == conferenceId);

                if (conference == null)
                    return false;

                var moderator = conference.Participants
                    .FirstOrDefault(p => p.UserId == moderatorId && (p.Role == "Moderator" || conference.OrganizerId == moderatorId));

                if (moderator == null)
                    throw new UnauthorizedAccessException("User not authorized to mute participants");

                var participant = conference.Participants
                    .FirstOrDefault(p => p.UserId == participantId);

                if (participant == null)
                    return false;

                participant.IsMuted = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Participant muted successfully. ConferenceId: {ConferenceId}, ParticipantId: {ParticipantId}, ModeratorId: {ModeratorId}", 
                    conferenceId, participantId, moderatorId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error muting participant. ConferenceId: {ConferenceId}, ParticipantId: {ParticipantId}, ModeratorId: {ModeratorId}", 
                    conferenceId, participantId, moderatorId);
                throw;
            }
        }

        public async Task<bool> KickParticipantAsync(Guid conferenceId, Guid participantId, Guid moderatorId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == conferenceId);

                if (conference == null)
                    return false;

                var moderator = conference.Participants
                    .FirstOrDefault(p => p.UserId == moderatorId && (p.Role == "Moderator" || conference.OrganizerId == moderatorId));

                if (moderator == null)
                    throw new UnauthorizedAccessException("User not authorized to kick participants");

                var participant = conference.Participants
                    .FirstOrDefault(p => p.UserId == participantId);

                if (participant == null)
                    return false;

                participant.LeftAt = DateTime.UtcNow;
                participant.IsKicked = true;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Participant kicked successfully. ConferenceId: {ConferenceId}, ParticipantId: {ParticipantId}, ModeratorId: {ModeratorId}", 
                    conferenceId, participantId, moderatorId);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error kicking participant. ConferenceId: {ConferenceId}, ParticipantId: {ParticipantId}, ModeratorId: {ModeratorId}", 
                    conferenceId, participantId, moderatorId);
                throw;
            }
        }

        public async Task<string> GenerateConferenceTokenAsync(Guid conferenceId, Guid userId)
        {
            try
            {
                var conference = await _context.VideoConferences
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == conferenceId);

                if (conference == null)
                    throw new ArgumentException("Conference not found");

                var participant = conference.Participants
                    .FirstOrDefault(p => p.UserId == userId);

                if (participant == null)
                    throw new UnauthorizedAccessException("User not authorized to join this conference");

                var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{conferenceId}:{userId}:{DateTime.UtcNow.Ticks}"));

                _logger.LogInformation("Conference token generated successfully. ConferenceId: {ConferenceId}, UserId: {UserId}", 
                    conferenceId, userId);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating conference token. ConferenceId: {ConferenceId}, UserId: {UserId}", 
                    conferenceId, userId);
                throw;
            }
        }
    }

    public enum ConferenceStatus
    {
        Scheduled,
        InProgress,
        Ended,
        Cancelled
    }
}
