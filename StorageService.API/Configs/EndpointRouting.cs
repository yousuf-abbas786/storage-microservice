using Asp.Versioning;
using System.Reflection;

namespace StorageService.API.Configs
{
    public static class EndpointRouting
    {
        public static RouteGroupBuilder MapGroup(this WebApplication app, EndpointGroupBase group)
        {
            var groupName = group.GetType().Name;
            var version = group.Version;
            var versionString = $"v{version.MajorVersion}";

            return app
                .MapGroup($"/api/v{version.MajorVersion}/{groupName}")
                .WithApiVersionSet(app.NewApiVersionSet()
                    .HasApiVersion(version)
                    .Build())
                .WithGroupName(versionString)
                .WithTags($"{groupName} - {versionString}")
                .WithOpenApi();
        }

        public static WebApplication MapEndpoints(this WebApplication app)
        {
            var endpointGroupType = typeof(EndpointGroupBase);

            var assembly = Assembly.GetExecutingAssembly();

            var endpointGroupTypes = assembly.GetExportedTypes()
                .Where(t => t.IsSubclassOf(endpointGroupType));

            foreach (var type in endpointGroupTypes)
            {
                if (Activator.CreateInstance(type) is EndpointGroupBase instance)
                {
                    instance.Map(app);
                }
            }

            return app;
        }
    }
}
