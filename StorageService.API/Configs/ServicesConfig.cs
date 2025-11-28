using StorageService.Application.Extensions;

namespace StorageService.API.Configs
{
    
    public static class ServicesConfig
    {
        public static void SetupServices(this IServiceCollection services)
        {
            services.AddServices();
        }
    }
}
