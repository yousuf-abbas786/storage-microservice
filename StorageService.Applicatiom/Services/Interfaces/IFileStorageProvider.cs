using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageService.Application.Services.Interfaces
{
    public interface IFileStorageProvider
    {
        Task<StoreFileResult> StoreAsync(
            Stream fileStream,
            string fileName,
            string? contentType,
            string container,
            CancellationToken cancellationToken = default);

        Task<Stream> GetAsync(
            string container,
            string fileKey,
            CancellationToken cancellationToken = default);

        Task<bool> DeleteAsync(
            string container,
            string fileKey,
            CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(
            string container,
            string fileKey,
            CancellationToken cancellationToken = default);
    }

    public class StoreFileResult
    {
        public string FileKey { get; init; } = default!;
        public long Length { get; init; }
    }
}
