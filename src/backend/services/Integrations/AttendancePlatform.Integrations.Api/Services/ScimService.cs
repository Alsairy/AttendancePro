using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using AttendancePlatform.Shared.Domain.DTOs;
using AttendancePlatform.Shared.Domain.Entities;
using AttendancePlatform.Shared.Infrastructure.Data;

namespace AttendancePlatform.Integrations.Api.Services
{
    public class ScimService
    {
        private readonly ILogger<ScimService> _logger;
        private readonly AttendancePlatformDbContext _context;

        public ScimService(ILogger<ScimService> logger, AttendancePlatformDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create user via SCIM");
                throw;
            }
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            try
            {
                _context.Roles.Add(role);
                await _context.SaveChangesAsync();
                return role;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create role via SCIM");
                throw;
            }
        }

        public async Task<User> GetUserAsync(Guid userId)
        {
            try
            {
                return await _context.Users.FindAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user via SCIM");
                throw;
            }
        }

        public async Task<Role> GetRoleAsync(Guid roleId)
        {
            try
            {
                return await _context.Roles.FindAsync(roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get role via SCIM");
                throw;
            }
        }
    }
}

