using System.Text.Json;

namespace StorageService.API.Configs.Results
{
    public class ResultCreated : IResult
    {
        private readonly string _location;
        private readonly object _data;

        public ResultCreated(string location, object data)
        {
            _location = location;
            _data = data;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status201Created;
            httpContext.Response.Headers.Location = _location;
            httpContext.Response.ContentType = "application/json";

            await JsonSerializer.SerializeAsync(
                httpContext.Response.Body,
                _data,
                _data.GetType());
        }
    }
}
