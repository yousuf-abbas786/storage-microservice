using Microsoft.AspNetCore.Http;

using StorageService.API.Models;

namespace StorageService.API.Configs.Results
{
    public class ResultOK : IResult
    {
        private readonly object res;
        public ResultOK(object _res)
        {
            res = _res;
        }

        public Task ExecuteAsync(HttpContext httpContext)
        {
            var result = new APIResult();

            if (res != null)
            {
                result.Data = res;
            }

            if (res == null)
            {
                result.Message = "No Data Found";
            }
            else if (res.Equals(true))
            {
                result.Message = "Data Created or Updated";
            }
            else if (res.Equals(false))
            {
                result.Message = "Data not Created or Updated";
            }


            return httpContext.Response.WriteAsJsonAsync<APIResult>(result);

        }
    }
}
