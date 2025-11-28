namespace StorageService.Application.DTOs
{
    public class FileListItem
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long Size { get; set; }
        public string Container { get; set; } = default!;
        public string? OwnerId { get; set; }
        public string? TenantId { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }
}

