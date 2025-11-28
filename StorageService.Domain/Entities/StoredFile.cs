

namespace StorageService.Domain.Entities
{
    public class StoredFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public long Size { get; set; }

        public string Container { get; set; } = "main";
        public string StorageKey { get; set; } = default!; // provider-specific key

        public string? Checksum { get; set; }
        public string? OwnerId { get; set; }
        public string? TenantId { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }

        // optional: JSON column with custom metadata
        public string? ExtraMetadataJson { get; set; }
    }
}
