using StorageService.Application.DTOs;

namespace StorageService.Application.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(string username, string password, string? tenantId, CancellationToken ct = default);
        Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default);
    }
}

