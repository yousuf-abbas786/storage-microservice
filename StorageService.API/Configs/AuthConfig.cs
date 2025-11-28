using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Infrastructure.Extensions;

namespace StorageService.API.Configs
{
    public static class AuthConfig
    {
        public static IServiceCollection SetupAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddAuthorization();
            return services;
        }
    }
}

