using Asp.Versioning;

namespace StorageService.API.Configs
{
    public abstract class EndpointGroupBase
    {
        public abstract void Map(WebApplication app);
        
        public ApiVersion Version { get; set; } = new ApiVersion(1, 0);
    }
}
