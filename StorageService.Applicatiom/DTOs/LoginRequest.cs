namespace StorageService.Application.DTOs
{
    public class LoginRequest
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string? TenantId { get; set; }
    }
}

