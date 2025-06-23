using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Collaboration.Api.Services
{
    public interface IScreenSharingService
    {
        Task<ScreenSharingSession> StartScreenSharingAsync(Guid userId, Guid? conferenceId = null, string? title = null);
        Task<bool> StopScreenSharingAsync(Guid sessionId, Guid userId);
        Task<ScreenSharingSession?> GetActiveSessionAsync(Guid sessionId);
        Task<IEnumerable<ScreenSharingSession>> GetUserSessionsAsync(Guid userId);
        Task<bool> JoinScreenSharingAsync(Guid sessionId, Guid userId);
        Task<bool> LeaveScreenSharingAsync(Guid sessionId, Guid userId);
        Task<IEnumerable<ScreenSharingParticipant>> GetSessionParticipantsAsync(Guid sessionId);
        Task<bool> GrantControlAsync(Guid sessionId, Guid participantId, Guid ownerId);
        Task<bool> RevokeControlAsync(Guid sessionId, Guid participantId, Guid ownerId);
        Task<string> GenerateSessionTokenAsync(Guid sessionId, Guid userId);
    }

    public class ScreenSharingService : IScreenSharingService
    {
        private readonly HudurDbContext _context;
        private readonly ILogger<ScreenSharingService> _logger;

        public ScreenSharingService(HudurDbContext context, ILogger<ScreenSharingService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ScreenSharingSession> StartScreenSharingAsync(Guid userId, Guid? conferenceId = null, string? title = null)
        {
            try
            {
                var session = new ScreenSharingSession
                {
                    Id = Guid.NewGuid(),
                    OwnerId = userId,
                    ConferenceId = conferenceId,
                    Title = title ?? "Screen Sharing Session",
                    StartedAt = DateTime.UtcNow,
                    Status = "Active",
                    SessionUrl = $"https://screen.hudu.sa/{Guid.NewGuid():N}",
                    IsRecording = false,
                    MaxParticipants = 50
                };

                _context.ScreenSharingSessions.Add(session);

                var participant = new ScreenSharingParticipant
                {
                    Id = Guid.NewGuid(),
                    SessionId = session.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                    Role = "Owner",
                    HasControl = true
                };

                _context.ScreenSharingParticipants.Add(participant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Screen sharing session started successfully. SessionId: {SessionId}, OwnerId: {OwnerId}", 
                    session.Id, userId);
                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting screen sharing session. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> StopScreenSharingAsync(Guid sessionId, Guid userId)
        {
            try
            {
                var session = await _context.ScreenSharingSessions
                    .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == userId);

                if (session == null)
                    return false;

                session.Status = "Ended";
                session.EndedAt = DateTime.UtcNow;

                var activeParticipants = await _context.ScreenSharingParticipants
                    .Where(p => p.SessionId == sessionId && p.LeftAt == null)
                    .ToListAsync();

                foreach (var participant in activeParticipants)
                {
                    participant.LeftAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Screen sharing session stopped successfully. SessionId: {SessionId}, OwnerId: {OwnerId}", 
                    sessionId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping screen sharing session. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);
                throw;
            }
        }

        public async Task<ScreenSharingSession?> GetActiveSessionAsync(Guid sessionId)
        {
            try
            {
                var session = await _context.ScreenSharingSessions
                    .Include(s => s.Owner)
                    .Include(s => s.Participants.Where(p => p.LeftAt == null))
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(s => s.Id == sessionId && s.Status == "Active");

                return session;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active screen sharing session. SessionId: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<IEnumerable<ScreenSharingSession>> GetUserSessionsAsync(Guid userId)
        {
            try
            {
                var sessions = await _context.ScreenSharingSessions
                    .Include(s => s.Participants)
                    .Where(s => s.OwnerId == userId || s.Participants.Any(p => p.UserId == userId))
                    .OrderByDescending(s => s.StartedAt)
                    .ToListAsync();

                return sessions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user screen sharing sessions. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> JoinScreenSharingAsync(Guid sessionId, Guid userId)
        {
            try
            {
                var session = await _context.ScreenSharingSessions
                    .Include(s => s.Participants)
                    .FirstOrDefaultAsync(s => s.Id == sessionId && s.Status == "Active");

                if (session == null)
                    return false;

                if (session.Participants.Count(p => p.LeftAt == null) >= session.MaxParticipants)
                    throw new InvalidOperationException("Session is full");

                var existingParticipant = session.Participants
                    .FirstOrDefault(p => p.UserId == userId && p.LeftAt == null);

                if (existingParticipant != null)
                    return true;

                var participant = new ScreenSharingParticipant
                {
                    Id = Guid.NewGuid(),
                    SessionId = sessionId,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                    Role = "Viewer",
                    HasControl = false
                };

                _context.ScreenSharingParticipants.Add(participant);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User joined screen sharing session successfully. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining screen sharing session. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);
                throw;
            }
        }

        public async Task<bool> LeaveScreenSharingAsync(Guid sessionId, Guid userId)
        {
            try
            {
                var participant = await _context.ScreenSharingParticipants
                    .FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == userId && p.LeftAt == null);

                if (participant == null)
                    return false;

                participant.LeftAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("User left screen sharing session successfully. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving screen sharing session. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<ScreenSharingParticipant>> GetSessionParticipantsAsync(Guid sessionId)
        {
            try
            {
                var participants = await _context.ScreenSharingParticipants
                    .Include(p => p.User)
                    .Where(p => p.SessionId == sessionId && p.LeftAt == null)
                    .ToListAsync();

                return participants;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving session participants. SessionId: {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<bool> GrantControlAsync(Guid sessionId, Guid participantId, Guid ownerId)
        {
            try
            {
                var session = await _context.ScreenSharingSessions
                    .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == ownerId);

                if (session == null)
                    throw new UnauthorizedAccessException("User not authorized to grant control");

                var participant = await _context.ScreenSharingParticipants
                    .FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == participantId && p.LeftAt == null);

                if (participant == null)
                    return false;

                var currentControllers = await _context.ScreenSharingParticipants
                    .Where(p => p.SessionId == sessionId && p.HasControl && p.UserId != ownerId)
                    .ToListAsync();

                foreach (var controller in currentControllers)
                {
                    controller.HasControl = false;
                }

                participant.HasControl = true;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Control granted successfully. SessionId: {SessionId}, ParticipantId: {ParticipantId}, OwnerId: {OwnerId}", 
                    sessionId, participantId, ownerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error granting control. SessionId: {SessionId}, ParticipantId: {ParticipantId}, OwnerId: {OwnerId}", 
                    sessionId, participantId, ownerId);
                throw;
            }
        }

        public async Task<bool> RevokeControlAsync(Guid sessionId, Guid participantId, Guid ownerId)
        {
            try
            {
                var session = await _context.ScreenSharingSessions
                    .FirstOrDefaultAsync(s => s.Id == sessionId && s.OwnerId == ownerId);

                if (session == null)
                    throw new UnauthorizedAccessException("User not authorized to revoke control");

                var participant = await _context.ScreenSharingParticipants
                    .FirstOrDefaultAsync(p => p.SessionId == sessionId && p.UserId == participantId);

                if (participant == null)
                    return false;

                participant.HasControl = false;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Control revoked successfully. SessionId: {SessionId}, ParticipantId: {ParticipantId}, OwnerId: {OwnerId}", 
                    sessionId, participantId, ownerId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking control. SessionId: {SessionId}, ParticipantId: {ParticipantId}, OwnerId: {OwnerId}", 
                    sessionId, participantId, ownerId);
                throw;
            }
        }

        public async Task<string> GenerateSessionTokenAsync(Guid sessionId, Guid userId)
        {
            try
            {
                var session = await _context.ScreenSharingSessions
                    .Include(s => s.Participants)
                    .FirstOrDefaultAsync(s => s.Id == sessionId);

                if (session == null)
                    throw new ArgumentException("Session not found");

                var participant = session.Participants
                    .FirstOrDefault(p => p.UserId == userId && p.LeftAt == null);

                if (participant == null)
                    throw new UnauthorizedAccessException("User not authorized to access this session");

                var token = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{sessionId}:{userId}:{DateTime.UtcNow.Ticks}"));

                _logger.LogInformation("Session token generated successfully. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating session token. SessionId: {SessionId}, UserId: {UserId}", 
                    sessionId, userId);
                throw;
            }
        }
    }
}
