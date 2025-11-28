using Microsoft.EntityFrameworkCore;
using StorageService.Application.Repositories;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.Infrastructure.Repositories
{
    public class StoredFileRepository : IStoredFileRepository
    {
        private readonly StorageDbContext _db;

        public StoredFileRepository(StorageDbContext db)
        {
            _db = db;
        }

        public async Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.StoredFiles
                .FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<StoredFile?> GetByIdNotDeletedAsync(Guid id, CancellationToken ct = default)
        {
            return await _db.StoredFiles
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id && x.DeletedAt == null, ct);
        }

        public async Task AddAsync(StoredFile file, CancellationToken ct = default)
        {
            await _db.StoredFiles.AddAsync(file, ct);
        }

        public async Task<bool> SaveChangesAsync(CancellationToken ct = default)
        {
            return await _db.SaveChangesAsync(ct) > 0;
        }
    }
}

