using StorageService.Application.DTOs;
using StorageService.Application.Repositories;
using StorageService.Application.Services.Interfaces;
using StorageService.Domain.Entities;

namespace StorageService.Application.Services
{
    public class FileService : IFileService
    {
        private readonly IStoredFileRepository _repository;
        private readonly IFileStorageProvider _storage;

        public FileService(IStoredFileRepository repository, IFileStorageProvider storage)
        {
            _repository = repository;
            _storage = storage;
        }

        public async Task<UploadFileResponse> UploadAsync(Stream stream, string fileName, string? contentType, string? ownerId, string? tenantId, CancellationToken ct = default)
        {
            var container = "main"; // or derive from tenant / config

            var stored = await _storage.StoreAsync(
                stream,
                fileName,
                contentType,
                container,
                ct);

            var entity = new StoredFile
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                ContentType = contentType ?? "application/octet-stream",
                Size = stored.Length,
                Container = container,
                StorageKey = stored.FileKey,
                OwnerId = ownerId,
                TenantId = tenantId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _repository.AddAsync(entity, ct);
            await _repository.SaveChangesAsync(ct);

            return new UploadFileResponse
            {
                Id = entity.Id,
                FileName = entity.FileName,
                ContentType = entity.ContentType,
                Size = entity.Size,
                Container = entity.Container,
                StorageKey = entity.StorageKey,
                CreatedAt = entity.CreatedAt
            };
        }

        public async Task<DownloadFileResult?> DownloadAsync(Guid id, CancellationToken ct = default)
        {
            var file = await _repository.GetByIdNotDeletedAsync(id, ct);

            if (file is null)
                return null;

            var stream = await _storage.GetAsync(file.Container, file.StorageKey, ct);

            return new DownloadFileResult
            {
                Stream = stream,
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Size
            };
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var file = await _repository.GetByIdAsync(id, ct);
            if (file is null)
                return false;

            var deletedFromStorage = await _storage.DeleteAsync(file.Container, file.StorageKey, ct);
            if (!deletedFromStorage)
                return false;

            file.DeletedAt = DateTimeOffset.UtcNow;
            await _repository.SaveChangesAsync(ct);
            return true;
        }
    }
}
