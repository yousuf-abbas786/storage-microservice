using StorageService.Application.DTOs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
