namespace StorageService.Application.DTOs
{
    public class LoginResponse
    {
        public string Token { get; set; } = default!;
        public DateTime ExpiresAt { get; set; }
    }
}

