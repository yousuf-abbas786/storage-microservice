

namespace StorageService.Application.DTOs
{
    public class UploadFileResponse
    {
        public Guid Id { get; set; }

        public string FileName { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public long Size { get; set; }

        public string Container { get; set; } = default!;

        public string StorageKey { get; set; } = default!;

        public DateTimeOffset CreatedAt { get; set; }
    }
}
