using Microsoft.EntityFrameworkCore;
using StorageService.Application.Repositories;
using StorageService.Application.Specifications;
using StorageService.Infrastructure.Data;
using System.Linq.Expressions;

namespace StorageService.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly StorageDbContext _db;
        protected readonly DbSet<T> _dbSet;

        public Repository(StorageDbContext db)
        {
            _db = db;
            _dbSet = db.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _dbSet.FindAsync(new object[] { id }, ct);
        }

        public virtual async Task<IReadOnlyList<T>> GetAsync(ISpecification<T> spec, CancellationToken ct = default)
        {
            return await ApplySpecification(spec).ToListAsync(ct);
        }

        public virtual async Task<int> CountAsync(ISpecification<T> spec, CancellationToken ct = default)
        {
            return await ApplySpecification(spec, skipPaging: true).CountAsync(ct);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await _dbSet.AddAsync(entity, ct);
            return entity;
        }

        public virtual async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _db.SaveChangesAsync(ct) > 0;
        }

        protected IQueryable<T> ApplySpecification(ISpecification<T> spec, bool skipPaging = false)
        {
            var query = _dbSet.AsQueryable();

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }

            query = spec.Includes
                .Aggregate(query, (current, include) => current.Include(include));

            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPagingEnabled && !skipPaging)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }

            return query;
        }
    }
}

