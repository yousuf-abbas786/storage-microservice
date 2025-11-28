using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Services;
using StorageService.Application.Services.Interfaces;

namespace StorageService.Application.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IAuthService, AuthService>();
            return services;
        }
    }
}
