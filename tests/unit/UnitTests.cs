using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using AttendancePlatform.Authentication.Api.Services;
using AttendancePlatform.Attendance.Api.Services;
using AttendancePlatform.FaceRecognition.Api.Services;
using AttendancePlatform.LeaveManagement.Api.Services;
using AttendancePlatform.Collaboration.Api.Services;
using AttendancePlatform.Analytics.Api.Services;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Infrastructure.Services;
using AttendancePlatform.Shared.Domain.Interfaces;
using System.Linq.Expressions;

namespace AttendancePlatform.Tests.Unit
{
    public class TestFixture : IDisposable
    {
        public AttendancePlatformDbContext Context { get; private set; }
        public Mock<ICacheService> CacheServiceMock { get; private set; }
        public Mock<ILogger<AuthenticationService>> AuthLoggerMock { get; private set; }
        public Mock<ILogger<AttendanceService>> AttendanceLoggerMock { get; private set; }
        public Mock<ILogger<FaceRecognitionService>> FaceLoggerMock { get; private set; }
        public Mock<ILogger<LeaveManagementService>> LeaveLoggerMock { get; private set; }
        public Mock<ILogger<ChatService>> ChatLoggerMock { get; private set; }
        public Mock<ILogger<Repository<User>>> RepositoryLoggerMock { get; private set; }
        public Mock<IJwtTokenService> JwtServiceMock { get; private set; }
        public Mock<ITwoFactorService> TwoFactorServiceMock { get; private set; }
        public Mock<IEmailService> EmailServiceMock { get; private set; }
        public Mock<IRefreshTokenService> RefreshTokenServiceMock { get; private set; }

        public TestFixture()
        {
            var options = new DbContextOptionsBuilder<AttendancePlatformDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            Context = new AttendancePlatformDbContext(options);
            
            CacheServiceMock = new Mock<ICacheService>();
            AuthLoggerMock = new Mock<ILogger<AuthenticationService>>();
            AttendanceLoggerMock = new Mock<ILogger<AttendanceService>>();
            FaceLoggerMock = new Mock<ILogger<FaceRecognitionService>>();
            LeaveLoggerMock = new Mock<ILogger<LeaveManagementService>>();
            ChatLoggerMock = new Mock<ILogger<ChatService>>();
            RepositoryLoggerMock = new Mock<ILogger<Repository<User>>>();
            JwtServiceMock = new Mock<IJwtTokenService>();
            TwoFactorServiceMock = new Mock<ITwoFactorService>();
            EmailServiceMock = new Mock<IEmailService>();
            RefreshTokenServiceMock = new Mock<IRefreshTokenService>();

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

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "test@example.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test123!"),
                FirstName = "Test",
                LastName = "User",
                IsActive = true,
                TenantId = tenant.Id,
                CreatedAt = DateTime.UtcNow
            };

            Context.Tenants.Add(tenant);
            Context.Users.Add(user);
            Context.SaveChanges();
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }

    public class AuthenticationServiceTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _fixture;
        private readonly AuthenticationService _service;

        public AuthenticationServiceTests(TestFixture fixture)
        {
            _fixture = fixture;
            _service = new AuthenticationService(
                _fixture.Context,
                _fixture.JwtServiceMock.Object,
                _fixture.AuthLoggerMock.Object,
                _fixture.TwoFactorServiceMock.Object,
                _fixture.EmailServiceMock.Object,
                _fixture.RefreshTokenServiceMock.Object
            );
        }

        [Fact]
        public async Task AuthenticateAsync_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var service = new AuthenticationService(_contextMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
            var email = "test@example.com";
            var password = "Test123!";

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                IsActive = true
            };

            _contextMock.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                      .ReturnsAsync(user);

            _jwtServiceMock.Setup(j => j.GenerateToken(It.IsAny<User>()))
                          .Returns("valid-jwt-token");

            // Act
            var result = await service.AuthenticateAsync(email, password);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("valid-jwt-token", result.Token);
            Assert.Equal(user.Id, result.UserId);
        }

        [Fact]
        public async Task AuthenticateAsync_WithInvalidCredentials_ReturnsNull()
        {
            // Arrange
            var service = new AuthenticationService(_contextMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
            var email = "test@example.com";
            var password = "WrongPassword";

            _contextMock.Setup(c => c.Users.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                      .ReturnsAsync((User)null);

            // Act
            var result = await service.AuthenticateAsync(email, password);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_WithValidData_CreatesUser()
        {
            // Arrange
            var service = new AuthenticationService(_contextMock.Object, _jwtServiceMock.Object, _loggerMock.Object);
            var registerDto = new RegisterDto
            {
                Email = "newuser@example.com",
                Password = "Test123!",
                FirstName = "Test",
                LastName = "User"
            };

            _contextMock.Setup(c => c.Users.AnyAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<User, bool>>>(), default))
                      .ReturnsAsync(false);

            _contextMock.Setup(c => c.Users.Add(It.IsAny<User>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await service.RegisterAsync(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.UserId);
        }
    }

    public class AttendanceServiceTests
    {
        private readonly Mock<ILogger<AttendanceService>> _loggerMock;
        private readonly Mock<AttendancePlatformDbContext> _contextMock;

        public AttendanceServiceTests()
        {
            _loggerMock = new Mock<ILogger<AttendanceService>>();
            _contextMock = new Mock<AttendancePlatformDbContext>();
        }

        [Fact]
        public async Task CheckInAsync_WithValidData_CreatesAttendanceRecord()
        {
            // Arrange
            var service = new AttendanceService(_contextMock.Object, _loggerMock.Object);
            var checkInDto = new CheckInDto
            {
                UserId = Guid.NewGuid(),
                CheckInTime = DateTime.UtcNow,
                Latitude = 40.7128,
                Longitude = -74.0060,
                Method = "GPS"
            };

            _contextMock.Setup(c => c.AttendanceRecords.Add(It.IsAny<AttendanceRecord>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await service.CheckInAsync(checkInDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.AttendanceId);
        }

        [Fact]
        public async Task CheckOutAsync_WithValidData_UpdatesAttendanceRecord()
        {
            // Arrange
            var service = new AttendanceService(_contextMock.Object, _loggerMock.Object);
            var userId = Guid.NewGuid();
            var checkOutDto = new CheckOutDto
            {
                UserId = userId,
                CheckOutTime = DateTime.UtcNow,
                Latitude = 40.7128,
                Longitude = -74.0060
            };

            var existingRecord = new AttendanceRecord
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CheckInTime = DateTime.UtcNow.AddHours(-8),
                CheckOutTime = null
            };

            _contextMock.Setup(c => c.AttendanceRecords.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<AttendanceRecord, bool>>>(), default))
                      .ReturnsAsync(existingRecord);

            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await service.CheckOutAsync(checkOutDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(existingRecord.CheckOutTime);
        }

        [Fact]
        public async Task GetAttendanceHistoryAsync_WithValidUser_ReturnsRecords()
        {
            // Arrange
            var service = new AttendanceService(_contextMock.Object, _loggerMock.Object);
            var userId = Guid.NewGuid();

            var attendanceRecords = new List<AttendanceRecord>
            {
                new AttendanceRecord { Id = Guid.NewGuid(), UserId = userId, CheckInTime = DateTime.UtcNow.AddDays(-1) },
                new AttendanceRecord { Id = Guid.NewGuid(), UserId = userId, CheckInTime = DateTime.UtcNow.AddDays(-2) }
            };

            _contextMock.Setup(c => c.AttendanceRecords.Where(It.IsAny<System.Linq.Expressions.Expression<System.Func<AttendanceRecord, bool>>>()))
                      .Returns(attendanceRecords.AsQueryable());

            // Act
            var result = await service.GetAttendanceHistoryAsync(userId, DateTime.UtcNow.AddDays(-7), DateTime.UtcNow);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }
    }

    public class FaceRecognitionServiceTests
    {
        private readonly Mock<ILogger<FaceRecognitionService>> _loggerMock;
        private readonly Mock<AttendancePlatformDbContext> _contextMock;

        public FaceRecognitionServiceTests()
        {
            _loggerMock = new Mock<ILogger<FaceRecognitionService>>();
            _contextMock = new Mock<AttendancePlatformDbContext>();
        }

        [Fact]
        public async Task EnrollFaceAsync_WithValidImage_CreatesTemplate()
        {
            // Arrange
            var service = new FaceRecognitionService(_contextMock.Object, _loggerMock.Object);
            var enrollDto = new FaceEnrollmentDto
            {
                UserId = Guid.NewGuid(),
                ImageData = "base64-encoded-image-data"
            };

            _contextMock.Setup(c => c.Biometrics.Add(It.IsAny<Biometrics>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await service.EnrollFaceAsync(enrollDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.TemplateId);
        }

        [Fact]
        public async Task VerifyFaceAsync_WithValidImage_ReturnsMatch()
        {
            // Arrange
            var service = new FaceRecognitionService(_contextMock.Object, _loggerMock.Object);
            var verifyDto = new FaceVerificationDto
            {
                ImageData = "base64-encoded-image-data"
            };

            var biometricTemplate = new Biometrics
            {
                Id = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                BiometricType = "Face",
                Template = "stored-face-template"
            };

            _contextMock.Setup(c => c.Biometrics.Where(It.IsAny<System.Linq.Expressions.Expression<System.Func<Biometrics, bool>>>()))
                      .Returns(new List<Biometrics> { biometricTemplate }.AsQueryable());

            // Act
            var result = await service.VerifyFaceAsync(verifyDto);

            // Assert
            Assert.True(result.IsMatch);
            Assert.NotNull(result.UserId);
        }
    }

    public class LeaveManagementServiceTests
    {
        private readonly Mock<ILogger<LeaveManagementService>> _loggerMock;
        private readonly Mock<AttendancePlatformDbContext> _contextMock;

        public LeaveManagementServiceTests()
        {
            _loggerMock = new Mock<ILogger<LeaveManagementService>>();
            _contextMock = new Mock<AttendancePlatformDbContext>();
        }

        [Fact]
        public async Task SubmitLeaveRequestAsync_WithValidData_CreatesRequest()
        {
            // Arrange
            var service = new LeaveManagementService(_contextMock.Object, _loggerMock.Object);
            var leaveRequestDto = new LeaveRequestDto
            {
                UserId = Guid.NewGuid(),
                LeaveType = "Annual",
                StartDate = DateTime.UtcNow.AddDays(7),
                EndDate = DateTime.UtcNow.AddDays(10),
                Reason = "Family vacation"
            };

            _contextMock.Setup(c => c.LeaveManagement.Add(It.IsAny<LeaveManagement>()));
            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await service.SubmitLeaveRequestAsync(leaveRequestDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.RequestId);
        }

        [Fact]
        public async Task ApproveLeaveRequestAsync_WithValidId_UpdatesStatus()
        {
            // Arrange
            var service = new LeaveManagementService(_contextMock.Object, _loggerMock.Object);
            var requestId = Guid.NewGuid();
            var approvalDto = new LeaveApprovalDto
            {
                RequestId = requestId,
                ApprovalStatus = "Approved",
                Comments = "Approved by manager"
            };

            var existingRequest = new LeaveManagement
            {
                Id = requestId,
                Status = "Pending",
                UserId = Guid.NewGuid()
            };

            _contextMock.Setup(c => c.LeaveManagement.FirstOrDefaultAsync(It.IsAny<System.Linq.Expressions.Expression<System.Func<LeaveManagement, bool>>>(), default))
                      .ReturnsAsync(existingRequest);

            _contextMock.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await service.ApproveLeaveRequestAsync(approvalDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Approved", existingRequest.Status);
        }
    }

    // DTOs for testing
    public class RegisterDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }

    public class CheckInDto
    {
        public Guid UserId { get; set; }
        public DateTime CheckInTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Method { get; set; } = string.Empty;
    }

    public class CheckOutDto
    {
        public Guid UserId { get; set; }
        public DateTime CheckOutTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class FaceEnrollmentDto
    {
        public Guid UserId { get; set; }
        public string ImageData { get; set; } = string.Empty;
    }

    public class FaceVerificationDto
    {
        public string ImageData { get; set; } = string.Empty;
    }

    public class LeaveRequestDto
    {
        public Guid UserId { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class LeaveApprovalDto
    {
        public Guid RequestId { get; set; }
        public string ApprovalStatus { get; set; } = string.Empty;
        public string Comments { get; set; } = string.Empty;
    }
}

