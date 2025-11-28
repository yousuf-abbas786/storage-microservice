using Microsoft.Extensions.DependencyInjection;
using StorageService.API.Middleware;

namespace StorageService.API.Configs
{
    public static class MiddlewareConfig
    {
        public static IServiceCollection SetupMiddleware(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();
            return services;
        }
    }
}

