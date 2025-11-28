using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StorageService.Application.DTOs;
using StorageService.Application.Repositories;
using StorageService.Application.Services.Interfaces;
using StorageService.Application.Specifications;
using StorageService.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StorageService.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IRepository<User> _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IRepository<User> userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<LoginResponse?> LoginAsync(string username, string password, string? tenantId, CancellationToken ct = default)
        {
            var spec = new GetUserByUsernameAndTenantSpecification(username, tenantId);
            var users = await _userRepository.GetAsync(spec, ct);
            var user = users.FirstOrDefault();

            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            user.LastLoginAt = DateTimeOffset.UtcNow;
            await _userRepository.SaveChangesAsync(ct);

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Token = token,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request, CancellationToken ct = default)
        {
            var existingSpec = new GetUserByUsernameAndTenantSpecification(request.Username, request.TenantId);
            var existing = await _userRepository.GetAsync(existingSpec, ct);

            if (existing.Any())
                throw new InvalidOperationException($"Username '{request.Username}' already exists for tenant '{request.TenantId}'");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = request.Username,
                Email = request.Email,
                PasswordHash = HashPassword(request.Password),
                TenantId = request.TenantId,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _userRepository.AddAsync(user, ct);
            await _userRepository.SaveChangesAsync(ct);

            return new RegisterResponse
            {
                UserId = user.Id,
                Username = user.Username,
                TenantId = user.TenantId
            };
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static bool VerifyPassword(string password, string passwordHash)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var hash = Convert.ToBase64String(hashedBytes);
            return hash == passwordHash;
        }

        public static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}

