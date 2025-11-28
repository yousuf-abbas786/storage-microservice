namespace StorageService.API.Configs.Results
{
    public static class ResultExtensions
    {
        public static IResult APIResult_Ok(this IResultExtensions resultExtensions, object data)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new ResultOK(data);
        }

        public static IResult APIResult_Error(this IResultExtensions resultExtensions)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new ResultError();
        }

        public static IResult APIResult_BadRequest(this IResultExtensions resultExtensions, string message)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new ResultBadRequest(message);
        }

        public static IResult APIResult_Created(this  IResultExtensions resultExtensions, string location, object data)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);

            return new ResultCreated(location, data);
        }

        public static IResult APIResult_NoContent(this IResultExtensions resultExtensions)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);
            return new ResultNoContent();
        }
        public static IResult APIResult_NotFound(this IResultExtensions resultExtensions)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);
            return new ResultNotFound();
        }

        public static IResult APIResult_NotFound(this IResultExtensions resultExtensions, string message)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);
            return new ResultNotFound(message);
        }

        public static IResult APIResult_File(this IResultExtensions resultExtensions, Stream stream, string contentType, string fileName, long? fileLength = null)
        {
            ArgumentNullException.ThrowIfNull(resultExtensions);
            ArgumentNullException.ThrowIfNull(stream);

            return new ResultFile(stream, contentType, fileName, fileLength);
        }

    }
}
