namespace StorageService.Application.DTOs
{
    public class RegisterResponse
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = default!;
        public string TenantId { get; set; } = default!;
        public string Message { get; set; } = "User registered successfully";
    }
}

