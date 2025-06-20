using Microsoft.EntityFrameworkCore;
using AttendancePlatform.Shared.Domain.Interfaces;
using AttendancePlatform.Shared.Infrastructure.Data;

namespace AttendancePlatform.Shared.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AttendancePlatformDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(AttendancePlatformDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            _dbSet.Update(entity);
            return await Task.FromResult(entity);
        }

        public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }

        public virtual async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, cancellationToken) != null;
        }
    }

    public class TenantRepository<T> : Repository<T>, ITenantRepository<T> 
        where T : class, ITenantAware
    {
        private readonly ITenantContext _tenantContext;

        public TenantRepository(AttendancePlatformDbContext context, ITenantContext tenantContext) 
            : base(context)
        {
            _tenantContext = tenantContext;
        }

        public virtual async Task<IEnumerable<T>> GetByTenantAsync(Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.Where(e => e.TenantId == tenantId).ToListAsync(cancellationToken);
        }

        public virtual async Task<T?> GetByIdAndTenantAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && e.TenantId == tenantId, cancellationToken);
        }

        public override async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            if (_tenantContext.TenantId.HasValue)
            {
                return await GetByTenantAsync(_tenantContext.TenantId.Value, cancellationToken);
            }
            
            return await base.GetAllAsync(cancellationToken);
        }

        public override async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (_tenantContext.TenantId.HasValue)
            {
                return await GetByIdAndTenantAsync(id, _tenantContext.TenantId.Value, cancellationToken);
            }
            
            return await base.GetByIdAsync(id, cancellationToken);
        }
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly AttendancePlatformDbContext _context;
        private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

        public UnitOfWork(AttendancePlatformDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync(cancellationToken);
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}

