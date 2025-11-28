using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Repositories;
using StorageService.Application.Services.Interfaces;
using StorageService.Infrastructure.Data;
using StorageService.Infrastructure.Repositories;

namespace StorageService.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageProviders(this IServiceCollection services)
        {
            services.AddSingleton<IFileStorageProvider, LocalFileStorageProvider>();
            return services;
        }

        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<StorageDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IStoredFileRepository, StoredFileRepository>();

            return services;
        }
    }
}
