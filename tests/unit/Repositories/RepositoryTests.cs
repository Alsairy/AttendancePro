using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Repositories;
using AttendancePlatform.Shared.Infrastructure.Services;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Tests.Unit.Repositories
{
    public class RepositoryTests : IDisposable
    {
        private readonly AttendancePlatformDbContext _context;
        private readonly Mock<ICacheService> _cacheServiceMock;
        private readonly Mock<ILogger<Repository<User>>> _loggerMock;
        private readonly Repository<User> _repository;

        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AttendancePlatformDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AttendancePlatformDbContext(options);
            _cacheServiceMock = new Mock<ICacheService>();
            _loggerMock = new Mock<ILogger<Repository<User>>>();
            
            _repository = new Repository<User>(_context, _cacheServiceMock.Object, _loggerMock.Object);

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

            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user1@test.com",
                    FirstName = "User",
                    LastName = "One",
                    TenantId = tenant.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user2@test.com",
                    FirstName = "User",
                    LastName = "Two",
                    TenantId = tenant.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _context.Tenants.Add(tenant);
            _context.Users.AddRange(users);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetByIdAsync_WhenCacheHit_ReturnsFromCache()
        {
            // Arrange
            var user = _context.Users.First();
            _cacheServiceMock.Setup(c => c.GetAsync<User>(It.IsAny<string>()))
                           .ReturnsAsync(user);

            // Act
            var result = await _repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            _cacheServiceMock.Verify(c => c.GetAsync<User>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCacheMiss_ReturnsFromDatabaseAndCaches()
        {
            // Arrange
            var user = _context.Users.First();
            _cacheServiceMock.Setup(c => c.GetAsync<User>(It.IsAny<string>()))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _repository.GetByIdAsync(user.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.Id);
            _cacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), It.IsAny<User>(), It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task GetPagedAsync_ReturnsCorrectPageSize()
        {
            // Arrange
            var page = 1;
            var pageSize = 1;
            _cacheServiceMock.Setup(c => c.GetAsync<List<User>>(It.IsAny<string>()))
                           .ReturnsAsync((List<User>)null);

            // Act
            var result = await _repository.GetPagedAsync(page, pageSize);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task FindAsync_WithPredicate_ReturnsMatchingEntities()
        {
            // Arrange
            var expectedEmail = "user1@test.com";

            // Act
            var result = await _repository.FindAsync(u => u.Email == expectedEmail);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(expectedEmail, result.First().Email);
        }

        [Fact]
        public async Task CountAsync_WhenCacheHit_ReturnsFromCache()
        {
            // Arrange
            var expectedCount = 5;
            _cacheServiceMock.Setup(c => c.GetAsync<int?>(It.IsAny<string>()))
                           .ReturnsAsync(expectedCount);

            // Act
            var result = await _repository.CountAsync();

            // Assert
            Assert.Equal(expectedCount, result);
            _cacheServiceMock.Verify(c => c.GetAsync<int?>(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_InvalidatesCacheAndAddsEntity()
        {
            // Arrange
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                Email = "newuser@test.com",
                FirstName = "New",
                LastName = "User",
                TenantId = _context.Tenants.First().Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            // Act
            var result = await _repository.AddAsync(newUser);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newUser.Id, result.Id);
            _cacheServiceMock.Verify(c => c.RemoveByPatternAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_InvalidatesCacheAndUpdatesEntity()
        {
            // Arrange
            var user = _context.Users.First();
            user.FirstName = "Updated";

            // Act
            var result = await _repository.UpdateAsync(user);
            await _context.SaveChangesAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated", result.FirstName);
            _cacheServiceMock.Verify(c => c.RemoveByPatternAsync(It.IsAny<string>()), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_InvalidatesCacheAndRemovesEntity()
        {
            // Arrange
            var user = _context.Users.First();
            var userId = user.Id;

            // Act
            await _repository.DeleteAsync(userId);
            await _context.SaveChangesAsync();

            // Assert
            var deletedUser = await _context.Users.FindAsync(userId);
            Assert.Null(deletedUser);
            _cacheServiceMock.Verify(c => c.RemoveByPatternAsync(It.IsAny<string>()), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Once);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
