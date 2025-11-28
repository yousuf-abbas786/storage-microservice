

namespace StorageService.Application.DTOs
{
    public class DownloadFileResult
    {
        public Stream Stream { get; init; } = default!;

        public string FileName { get; init; } = default!;

        public string ContentType { get; init; } = default!;

        // Optional, but recommended:
        public long? Size { get; init; }
    }
}
