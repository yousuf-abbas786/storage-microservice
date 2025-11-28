namespace StorageService.API.Configs.Results
{
    public class ResultFile : IResult
    {
        private readonly Stream _stream;
        private readonly string _contentType;
        private readonly string _fileName;
        private readonly long? _fileLength;

        public ResultFile(Stream stream, string contentType, string fileName, long? fileLength = null)
        {
            _stream = stream;
            _contentType = contentType;
            _fileName = fileName;
            _fileLength = fileLength;
        }

        public async Task ExecuteAsync(HttpContext httpContext)
        {
            httpContext.Response.ContentType = _contentType;
            httpContext.Response.Headers.ContentDisposition = $"attachment; filename=\"{_fileName}\"";

            if (_fileLength.HasValue)
            {
                httpContext.Response.ContentLength = _fileLength.Value;
            }

            try
            {
                await _stream.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);
                await httpContext.Response.Body.FlushAsync(httpContext.RequestAborted);
            }
            finally
            {
                await _stream.DisposeAsync();
            }
        }
    }
}
