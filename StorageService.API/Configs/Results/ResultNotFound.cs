using System.Text.Json;

namespace StorageService.API.Configs.Results
{
    public class ResultNotFound : IResult
    {
        private readonly string? _message;

        public ResultNotFound()
        {
        }

        public ResultNotFound(string message)
        {
            _message = message;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

            if (!string.IsNullOrWhiteSpace(_message))
            {
                httpContext.Response.ContentType = "application/json";

                var response = new
                {
                    error = _message,
                    status = 404
                };

                await JsonSerializer.SerializeAsync(
                    httpContext.Response.Body,
                    response);
            }
        }
    }
}
