using Microsoft.EntityFrameworkCore;
using Xunit;
using FluentAssertions;
using AttendancePlatform.Shared.Infrastructure.Data;

namespace AttendancePlatform.Tests.Database
{
    public class DatabaseMigrationTests : DatabaseTestBase
    {
        [Fact]
        public async Task Database_ShouldCreateAllTablesSuccessfully()
        {
            var tableNames = new[]
            {
                "Tenants",
                "Users",
                "AttendanceRecords",
                "LeaveRequests",
                "Notifications",
                "AuditLogs",
                "Geofences",
                "Kiosks",
                "BiometricTemplates"
            };

            foreach (var tableName in tableNames)
            {
                var tableExists = await DbContext.Database
                    .SqlQueryRaw<int>($"SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '{tableName}'")
                    .FirstOrDefaultAsync();

                tableExists.Should().Be(1, $"Table {tableName} should exist");
            }
        }

        [Fact]
        public async Task Database_ShouldHaveCorrectIndexes()
        {
            var indexQueries = new[]
            {
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_Users_Email'",
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_Users_TenantId'",
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_AttendanceRecords_UserId'",
                "SELECT COUNT(*) FROM sys.indexes WHERE name = 'IX_AttendanceRecords_TenantId'"
            };

            foreach (var query in indexQueries)
            {
                var indexExists = await DbContext.Database
                    .SqlQueryRaw<int>(query)
                    .FirstOrDefaultAsync();

                indexExists.Should().BeGreaterThan(0, $"Index should exist for query: {query}");
            }
        }

        [Fact]
        public async Task Database_ShouldEnforceForeignKeyConstraints()
        {
            await SeedTestDataAsync();

            var invalidUserId = Guid.NewGuid();
            var attendanceRecord = new AttendancePlatform.Shared.Domain.Entities.AttendanceRecord
            {
                Id = Guid.NewGuid(),
                UserId = invalidUserId,
                TenantId = Guid.NewGuid(),
                CheckInTime = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            DbContext.AttendanceRecords.Add(attendanceRecord);

            var act = async () => await DbContext.SaveChangesAsync();
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task Database_ShouldEnforceUniqueConstraints()
        {
            await SeedTestDataAsync();

            var existingUser = await DbContext.Users.FirstAsync();
            var duplicateUser = new AttendancePlatform.Shared.Domain.Entities.User
            {
                Id = Guid.NewGuid(),
                TenantId = existingUser.TenantId,
                Email = existingUser.Email,
                FirstName = "Duplicate",
                LastName = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            DbContext.Users.Add(duplicateUser);

            var act = async () => await DbContext.SaveChangesAsync();
            await act.Should().ThrowAsync<DbUpdateException>();
        }

        [Fact]
        public async Task Database_ShouldSupportConcurrentOperations()
        {
            await SeedTestDataAsync();

            var user = await DbContext.Users.FirstAsync();
            var tasks = new List<Task>();

            for (int i = 0; i < 10; i++)
            {
                var taskIndex = i;
                tasks.Add(Task.Run(async () =>
                {
                    using var scope = new ServiceCollection()
                        .AddDbContext<AttendancePlatformDbContext>(options =>
                            options.UseSqlServer(DbContext.Database.GetConnectionString()))
                        .BuildServiceProvider()
                        .CreateScope();

                    var context = scope.ServiceProvider.GetRequiredService<AttendancePlatformDbContext>();
                    
                    var attendanceRecord = new AttendancePlatform.Shared.Domain.Entities.AttendanceRecord
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        TenantId = user.TenantId,
                        CheckInTime = DateTime.UtcNow.AddMinutes(taskIndex),
                        CreatedAt = DateTime.UtcNow
                    };

                    context.AttendanceRecords.Add(attendanceRecord);
                    await context.SaveChangesAsync();
                }));
            }

            await Task.WhenAll(tasks);

            var recordCount = await DbContext.AttendanceRecords.CountAsync();
            recordCount.Should().Be(10);
        }

        [Fact]
        public async Task Database_ShouldHandleLargeDataSets()
        {
            await SeedTestDataAsync();

            var user = await DbContext.Users.FirstAsync();
            var attendanceRecords = new List<AttendancePlatform.Shared.Domain.Entities.AttendanceRecord>();

            for (int i = 0; i < 1000; i++)
            {
                attendanceRecords.Add(new AttendancePlatform.Shared.Domain.Entities.AttendanceRecord
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    TenantId = user.TenantId,
                    CheckInTime = DateTime.UtcNow.AddDays(-i),
                    CreatedAt = DateTime.UtcNow
                });
            }

            DbContext.AttendanceRecords.AddRange(attendanceRecords);
            await DbContext.SaveChangesAsync();

            var count = await DbContext.AttendanceRecords.CountAsync();
            count.Should().Be(1000);

            var recentRecords = await DbContext.AttendanceRecords
                .Where(r => r.CheckInTime >= DateTime.UtcNow.AddDays(-30))
                .CountAsync();

            recentRecords.Should().BeLessOrEqualTo(30);
        }

        [Fact]
        public async Task Database_ShouldSupportTransactions()
        {
            await SeedTestDataAsync();

            var user = await DbContext.Users.FirstAsync();

            using var transaction = await DbContext.Database.BeginTransactionAsync();

            try
            {
                var attendanceRecord = new AttendancePlatform.Shared.Domain.Entities.AttendanceRecord
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    TenantId = user.TenantId,
                    CheckInTime = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                DbContext.AttendanceRecords.Add(attendanceRecord);
                await DbContext.SaveChangesAsync();

                var auditLog = new AttendancePlatform.Shared.Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    Action = "CheckIn",
                    EntityType = "AttendanceRecord",
                    EntityId = attendanceRecord.Id.ToString(),
                    Timestamp = DateTime.UtcNow
                };

                DbContext.AuditLogs.Add(auditLog);
                await DbContext.SaveChangesAsync();

                await transaction.CommitAsync();

                var recordExists = await DbContext.AttendanceRecords.AnyAsync(r => r.Id == attendanceRecord.Id);
                var auditExists = await DbContext.AuditLogs.AnyAsync(a => a.Id == auditLog.Id);

                recordExists.Should().BeTrue();
                auditExists.Should().BeTrue();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
