using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StorageService.Application.Options;

namespace StorageService.API.Configs
{
    public static class OptionsConfig
    {
        public static IServiceCollection SetupOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageOptions>(
                configuration.GetSection(StorageOptions.SectionName));
            
            services.Configure<JwtOptions>(
                configuration.GetSection(JwtOptions.SectionName));

            return services;
        }
    }
}

