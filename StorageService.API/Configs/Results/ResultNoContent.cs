namespace StorageService.API.Configs.Results
{
    public class ResultNoContent : IResult
    {
        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.StatusCode = StatusCodes.Status204NoContent;
            await Task.CompletedTask;
        }
    }
}
