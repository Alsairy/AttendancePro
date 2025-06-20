using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Collaboration.Api.Services;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;

namespace AttendancePlatform.Tests.Unit.Services
{
    public class CollaborationServiceTests : IDisposable
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly Mock<ILogger<ChatService>> _chatLoggerMock;
        private readonly Mock<ILogger<TeamCollaborationService>> _teamLoggerMock;
        private readonly ChatService _chatService;
        private readonly TeamCollaborationService _teamService;

        public CollaborationServiceTests()
        {
            var options = new DbContextOptionsBuilder<AttendancePlatformDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AttendancePlatformDbContext(options);
            _chatLoggerMock = new Mock<ILogger<ChatService>>();
            _teamLoggerMock = new Mock<ILogger<TeamCollaborationService>>();
            
            _chatService = new ChatService(_context, _chatLoggerMock.Object);
            _teamService = new TeamCollaborationService(_context, _teamLoggerMock.Object);

            SeedTestData();
        }

        private void SeedTestData()
        {
            var tenant = new Tenant
            {
                Id = Guid.NewGuid(),
                Name = "Test Tenant",
                Subdomain = "test",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var user1 = new User
            {
                Id = Guid.NewGuid(),
                Email = "user1@test.com",
                FirstName = "User",
                LastName = "One",
                TenantId = tenant.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var user2 = new User
            {
                Id = Guid.NewGuid(),
                Email = "user2@test.com",
                FirstName = "User",
                LastName = "Two",
                TenantId = tenant.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            _context.Users.AddRange(user1, user2);
            _context.SaveChanges();
        }

        [Fact]
        public async Task CreateTeamAsync_WithValidData_CreatesTeamAndOwnerMembership()
        {
            // Arrange
            var user = _context.Users.First();
            var teamName = "Test Team";
            var description = "Test team description";

            // Act
            var result = await _teamService.CreateTeamAsync(teamName, description, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(teamName, result.Name);
            Assert.Equal(description, result.Description);
            Assert.Equal(user.Id, result.CreatedById);

            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == result.Id && tm.UserId == user.Id);
            Assert.NotNull(teamMember);
            Assert.Equal("Owner", teamMember.Role);
        }

        [Fact]
        public async Task AddTeamMemberAsync_WithValidData_AddsTeamMember()
        {
            // Arrange
            var users = _context.Users.ToList();
            var owner = users[0];
            var member = users[1];
            
            var team = await _teamService.CreateTeamAsync("Test Team", "Description", owner.Id);

            // Act
            var result = await _teamService.AddTeamMemberAsync(team.Id, member.Id, "Member");

            // Assert
            Assert.True(result);
            
            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(tm => tm.TeamId == team.Id && tm.UserId == member.Id);
            Assert.NotNull(teamMember);
            Assert.Equal("Member", teamMember.Role);
        }

        [Fact]
        public async Task CreateChannelAsync_WithValidData_CreatesChannel()
        {
            // Arrange
            var user = _context.Users.First();
            var channelName = "Test Channel";
            var description = "Test channel description";

            // Act
            var result = await _chatService.CreateChannelAsync(channelName, description, user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(channelName, result.Name);
            Assert.Equal(description, result.Description);
            Assert.Equal(user.Id, result.CreatedById);
        }

        [Fact]
        public async Task SendMessageAsync_WithValidData_CreatesMessage()
        {
            // Arrange
            var user = _context.Users.First();
            var channel = await _chatService.CreateChannelAsync("Test Channel", "Description", user.Id);
            var messageContent = "Hello, world!";

            // Act
            var result = await _chatService.SendMessageAsync(channel.Id, user.Id, messageContent);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(messageContent, result.Content);
            Assert.Equal(user.Id, result.SenderId);
            Assert.Equal(channel.Id, result.ChannelId);
        }

        [Fact]
        public async Task GetMessagesAsync_WithValidChannel_ReturnsMessages()
        {
            // Arrange
            var user = _context.Users.First();
            var channel = await _chatService.CreateChannelAsync("Test Channel", "Description", user.Id);
            
            await _chatService.SendMessageAsync(channel.Id, user.Id, "Message 1");
            await _chatService.SendMessageAsync(channel.Id, user.Id, "Message 2");

            // Act
            var result = await _chatService.GetMessagesAsync(channel.Id, 1, 10);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
