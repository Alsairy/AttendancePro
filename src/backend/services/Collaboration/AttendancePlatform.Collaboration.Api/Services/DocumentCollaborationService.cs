using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Infrastructure.Data;
using AttendancePlatform.Shared.Domain.Entities;

namespace AttendancePlatform.Collaboration.Api.Services
{
    public interface IDocumentCollaborationService
    {
        Task<CollaborativeDocument> CreateDocumentAsync(string title, string content, Guid createdById, Guid? teamId = null);
        Task<CollaborativeDocument> UpdateDocumentAsync(Guid documentId, string content, Guid userId);
        Task<CollaborativeDocument?> GetDocumentAsync(Guid documentId);
        Task<IEnumerable<CollaborativeDocument>> GetUserDocumentsAsync(Guid userId);
        Task<bool> ShareDocumentAsync(Guid documentId, Guid userId, string permission = "Read");
        Task<bool> RevokeDocumentAccessAsync(Guid documentId, Guid userId);
        Task<IEnumerable<DocumentVersion>> GetDocumentVersionsAsync(Guid documentId);
        Task<DocumentVersion> CreateDocumentVersionAsync(Guid documentId, string content, Guid userId, string? comment = null);
        Task<bool> LockDocumentAsync(Guid documentId, Guid userId);
        Task<bool> UnlockDocumentAsync(Guid documentId, Guid userId);
    }

    public class DocumentCollaborationService : IDocumentCollaborationService
    {
        private readonly HudurDbContext _context;
        private readonly ILogger<DocumentCollaborationService> _logger;

        public DocumentCollaborationService(HudurDbContext context, ILogger<DocumentCollaborationService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CollaborativeDocument> CreateDocumentAsync(string title, string content, Guid createdById, Guid? teamId = null)
        {
            try
            {
                var document = new CollaborativeDocument
                {
                    Id = Guid.NewGuid(),
                    Title = title,
                    Content = content,
                    CreatedById = createdById,
                    TeamId = teamId,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow,
                    LastModifiedById = createdById,
                    IsLocked = false,
                    Version = 1
                };

                _context.CollaborativeDocuments.Add(document);

                var permission = new DocumentPermission
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    UserId = createdById,
                    Permission = "Owner",
                    GrantedAt = DateTime.UtcNow
                };

                _context.DocumentPermissions.Add(permission);

                var version = new DocumentVersion
                {
                    Id = Guid.NewGuid(),
                    DocumentId = document.Id,
                    Content = content,
                    Version = 1,
                    CreatedById = createdById,
                    CreatedAt = DateTime.UtcNow,
                    Comment = "Initial version"
                };

                _context.DocumentVersions.Add(version);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document created successfully. DocumentId: {DocumentId}, Title: {Title}", 
                    document.Id, title);
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document. Title: {Title}, CreatedById: {CreatedById}", title, createdById);
                throw;
            }
        }

        public async Task<CollaborativeDocument> UpdateDocumentAsync(Guid documentId, string content, Guid userId)
        {
            try
            {
                var document = await _context.CollaborativeDocuments
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                if (document == null)
                    throw new ArgumentException("Document not found");

                if (document.IsLocked && document.LockedById != userId)
                    throw new UnauthorizedAccessException("Document is locked by another user");

                var hasPermission = await _context.DocumentPermissions
                    .AnyAsync(dp => dp.DocumentId == documentId && dp.UserId == userId && 
                             (dp.Permission == "Write" || dp.Permission == "Owner"));

                if (!hasPermission)
                    throw new UnauthorizedAccessException("User does not have write permission");

                document.Content = content;
                document.LastModifiedAt = DateTime.UtcNow;
                document.LastModifiedById = userId;
                document.Version++;

                var version = new DocumentVersion
                {
                    Id = Guid.NewGuid(),
                    DocumentId = documentId,
                    Content = content,
                    Version = document.Version,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    Comment = "Document updated"
                };

                _context.DocumentVersions.Add(version);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document updated successfully. DocumentId: {DocumentId}, Version: {Version}, UserId: {UserId}", 
                    documentId, document.Version, userId);
                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating document. DocumentId: {DocumentId}, UserId: {UserId}", documentId, userId);
                throw;
            }
        }

        public async Task<CollaborativeDocument?> GetDocumentAsync(Guid documentId)
        {
            try
            {
                var document = await _context.CollaborativeDocuments
                    .Include(d => d.CreatedBy)
                    .Include(d => d.LastModifiedBy)
                    .Include(d => d.Permissions)
                    .ThenInclude(p => p.User)
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                return document;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document. DocumentId: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<IEnumerable<CollaborativeDocument>> GetUserDocumentsAsync(Guid userId)
        {
            try
            {
                var documents = await _context.DocumentPermissions
                    .Include(dp => dp.Document)
                    .ThenInclude(d => d.CreatedBy)
                    .Where(dp => dp.UserId == userId)
                    .Select(dp => dp.Document)
                    .OrderByDescending(d => d.LastModifiedAt)
                    .ToListAsync();

                return documents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user documents. UserId: {UserId}", userId);
                throw;
            }
        }

        public async Task<bool> ShareDocumentAsync(Guid documentId, Guid userId, string permission = "Read")
        {
            try
            {
                var existingPermission = await _context.DocumentPermissions
                    .FirstOrDefaultAsync(dp => dp.DocumentId == documentId && dp.UserId == userId);

                if (existingPermission != null)
                {
                    existingPermission.Permission = permission;
                }
                else
                {
                    var newPermission = new DocumentPermission
                    {
                        Id = Guid.NewGuid(),
                        DocumentId = documentId,
                        UserId = userId,
                        Permission = permission,
                        GrantedAt = DateTime.UtcNow
                    };

                    _context.DocumentPermissions.Add(newPermission);
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Document shared successfully. DocumentId: {DocumentId}, UserId: {UserId}, Permission: {Permission}", 
                    documentId, userId, permission);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sharing document. DocumentId: {DocumentId}, UserId: {UserId}", documentId, userId);
                throw;
            }
        }

        public async Task<bool> RevokeDocumentAccessAsync(Guid documentId, Guid userId)
        {
            try
            {
                var permission = await _context.DocumentPermissions
                    .FirstOrDefaultAsync(dp => dp.DocumentId == documentId && dp.UserId == userId);

                if (permission == null || permission.Permission == "Owner")
                    return false;

                _context.DocumentPermissions.Remove(permission);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document access revoked successfully. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking document access. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                throw;
            }
        }

        public async Task<IEnumerable<DocumentVersion>> GetDocumentVersionsAsync(Guid documentId)
        {
            try
            {
                var versions = await _context.DocumentVersions
                    .Include(dv => dv.CreatedBy)
                    .Where(dv => dv.DocumentId == documentId)
                    .OrderByDescending(dv => dv.Version)
                    .ToListAsync();

                return versions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving document versions. DocumentId: {DocumentId}", documentId);
                throw;
            }
        }

        public async Task<DocumentVersion> CreateDocumentVersionAsync(Guid documentId, string content, Guid userId, string? comment = null)
        {
            try
            {
                var document = await _context.CollaborativeDocuments
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                if (document == null)
                    throw new ArgumentException("Document not found");

                var version = new DocumentVersion
                {
                    Id = Guid.NewGuid(),
                    DocumentId = documentId,
                    Content = content,
                    Version = document.Version + 1,
                    CreatedById = userId,
                    CreatedAt = DateTime.UtcNow,
                    Comment = comment ?? "Manual version created"
                };

                document.Version = version.Version;
                document.Content = content;
                document.LastModifiedAt = DateTime.UtcNow;
                document.LastModifiedById = userId;

                _context.DocumentVersions.Add(version);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Document version created successfully. DocumentId: {DocumentId}, Version: {Version}, UserId: {UserId}", 
                    documentId, version.Version, userId);
                return version;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating document version. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                throw;
            }
        }

        public async Task<bool> LockDocumentAsync(Guid documentId, Guid userId)
        {
            try
            {
                var document = await _context.CollaborativeDocuments
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                if (document == null)
                    return false;

                if (document.IsLocked)
                    return false;

                document.IsLocked = true;
                document.LockedById = userId;
                document.LockedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Document locked successfully. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error locking document. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                throw;
            }
        }

        public async Task<bool> UnlockDocumentAsync(Guid documentId, Guid userId)
        {
            try
            {
                var document = await _context.CollaborativeDocuments
                    .FirstOrDefaultAsync(d => d.Id == documentId);

                if (document == null)
                    return false;

                if (!document.IsLocked || document.LockedById != userId)
                    return false;

                document.IsLocked = false;
                document.LockedById = null;
                document.LockedAt = null;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Document unlocked successfully. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unlocking document. DocumentId: {DocumentId}, UserId: {UserId}", 
                    documentId, userId);
                throw;
            }
        }
    }
}
