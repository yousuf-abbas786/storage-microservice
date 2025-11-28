using StorageService.Domain.Entities;

namespace StorageService.Application.Repositories
{
    public interface IStoredFileRepository
    {
        Task<StoredFile?> GetByIdAsync(Guid id, CancellationToken ct = default);
        Task<StoredFile?> GetByIdNotDeletedAsync(Guid id, CancellationToken ct = default);
        Task AddAsync(StoredFile file, CancellationToken ct = default);
        Task<bool> SaveChangesAsync(CancellationToken ct = default);
    }
}

