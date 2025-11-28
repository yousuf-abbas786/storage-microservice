using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using StorageService.Application.Options;
using StorageService.Application.Repositories;
using StorageService.Application.Services.Interfaces;
using StorageService.Domain.Entities;
using StorageService.Infrastructure.Data;
using StorageService.Infrastructure.Repositories;
using System.Text;

namespace StorageService.Infrastructure.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection AddStorageProviders(this IServiceCollection services, IConfiguration configuration)
        {
            var storageOptions = configuration.GetSection(StorageOptions.SectionName)
                .Get<StorageOptions>()
                ?? throw new InvalidOperationException("Storage configuration not found.");

            var provider = storageOptions.Provider?.Trim() ?? "Local";

            switch (provider.ToLowerInvariant())
            {
                case "local":
                    services.AddSingleton<IFileStorageProvider, LocalFileStorageProvider>();
                    break;

                default:
                    throw new InvalidOperationException(
                        $"Unknown storage provider: {provider}. Supported: Local");
            }

            return services;
        }

        public static IServiceCollection AddDataAccess(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<StorageDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IStoredFileRepository, StoredFileRepository>();
            services.AddScoped<IRepository<User>, UserRepository>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

            services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()
                    ?? throw new InvalidOperationException("JWT configuration not found.");

                options.TokenValidationParameters.ValidIssuer = jwtOptions.Issuer;
                options.TokenValidationParameters.ValidAudience = jwtOptions.Audience;
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key));
            });

            return services;
        }
    }
}
