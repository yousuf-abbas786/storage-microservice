using StorageService.Application.DTOs;

namespace StorageService.Application.Services.Interfaces
{
    public interface IFileService
    {
        Task<UploadFileResponse> UploadAsync(
            Stream stream,
            string fileName,
            string? contentType,
            string? ownerId,
            string? tenantId,
            CancellationToken ct = default);

        Task<DownloadFileResult?> DownloadAsync(Guid id, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
        Task<PagedResult<FileListItem>> GetFilesAsync(PaginationRequest request, CancellationToken ct = default);
        Task<PagedResult<FileListItem>> SearchFilesAsync(string fileName, PaginationRequest request, CancellationToken ct = default);
    }
}
