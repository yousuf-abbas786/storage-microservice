using Microsoft.EntityFrameworkCore;
using StorageService.Application.Repositories;
using StorageService.Application.Specifications;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Repositories
{
    public class StoredFileRepository : Repository<StoredFile>, IStoredFileRepository
    {
        public StoredFileRepository(StorageDbContext db) : base(db)
        {
        }

        public async Task<StoredFile?> GetByIdNotDeletedAsync(Guid id, CancellationToken ct = default)
        {
            var spec = new GetFileByIdSpecification(id);
            var results = await GetAsync(spec, ct);
            ct.ThrowIfCancellationRequested();
            return results.FirstOrDefault();
        }
    }
}

