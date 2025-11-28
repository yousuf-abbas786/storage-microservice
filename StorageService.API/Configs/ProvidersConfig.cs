using StorageService.Infrastructure.Extensions;

namespace StorageService.API.Configs
{
    public static class ProvidersConfig
    {
        public static void SetupProviders(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStorageProviders(configuration);
            services.AddDataAccess(configuration);
        }
    }
}
