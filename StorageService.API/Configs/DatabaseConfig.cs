using Microsoft.EntityFrameworkCore;
using Serilog;
using StorageService.Application.Repositories;
using StorageService.Application.Services;
using StorageService.Application.Specifications;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;

namespace StorageService.API.Configs
{
    public static class DatabaseConfig
    {
        public static async Task SetupDatabaseAsync(this WebApplication app)
        {
            await using var scope = app.Services.CreateAsyncScope();
            
            var db = scope.ServiceProvider.GetRequiredService<StorageDbContext>();
            db.Database.Migrate();

            await SetupDefaultUserAsync(scope);
        }

        private static async Task SetupDefaultUserAsync(IServiceScope scope)
        {
            var userRepository = scope.ServiceProvider.GetRequiredService<IRepository<User>>();
            var hasUsers = await userRepository.GetAsync(
                new GetAllUsersSpecification(),
                CancellationToken.None);

            if (!hasUsers.Any())
            {
                var defaultUser = new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@storage.com",
                    PasswordHash = AuthService.HashPassword("123"),
                    TenantId = "yousuf",
                    CreatedAt = DateTimeOffset.UtcNow
                };

                await userRepository.AddAsync(defaultUser, CancellationToken.None);
                await userRepository.SaveChangesAsync(CancellationToken.None);

                Log.Information("Default admin user created: admin@storage.com (tenant: yousuf)");
            }
        }
    }
}

