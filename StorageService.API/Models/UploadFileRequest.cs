using Microsoft.AspNetCore.Http;

namespace StorageService.API.Models
{
    public class UploadFileRequest
    {
        public IFormFile File { get; set; } = default!;
        public string? OwnerId { get; set; }
        public string? TenantId { get; set; }
    }
}

