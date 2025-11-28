using StorageService.Application.Specifications;
using StorageService.Domain.Entities;

namespace StorageService.Application.Repositories
{
    public interface IStoredFileRepository : IRepository<StoredFile>
    {
        Task<StoredFile?> GetByIdNotDeletedAsync(Guid id, CancellationToken ct = default);
    }
}

