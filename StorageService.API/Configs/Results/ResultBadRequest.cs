using StorageService.API.Models;

using System.Net;

namespace StorageService.API.Configs.Results
{
    public class ResultBadRequest : IResult
    {
        private readonly string message;
        public ResultBadRequest(string _message)
        {
            message = _message;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            var result = new APIResult
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = message,
                Data = null
            };


            return httpContext.Response.WriteAsJsonAsync<APIResult>(result);
        }
    }
}
