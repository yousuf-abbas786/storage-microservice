using Microsoft.AspNetCore.Mvc;
using StorageService.API.Configs;
using StorageService.API.Configs.Results;
using StorageService.API.Models;
using StorageService.Application.DTOs;
using StorageService.Application.Services.Interfaces;

namespace StorageService.API.Endpoints
{
    public class Files : EndpointGroupBase
    {
        public override void Map(WebApplication app)
        {
            var group = app.MapGroup(this);


            // POST /api/files  -> upload a file
            group.MapPost("/", UploadFileAsync)
                 .WithName("UploadFile")
                 .DisableAntiforgery()
                 .Accepts<UploadFileRequest>("multipart/form-data")
                 .RequireAuthorization()
                 .Produces<UploadFileResponse>(StatusCodes.Status201Created)
                 .ProducesProblem(StatusCodes.Status400BadRequest)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

            // GET /api/files -> get paginated list of files (must be before /{id} route)
            group.MapGet("/", GetFilesAsync)
                 .WithName("GetFiles")
                 .RequireAuthorization()
                 .Produces<PagedResult<FileListItem>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

            // GET /api/files/search?fileName=... -> search files by name
            group.MapGet("/search", SearchFilesAsync)
                 .WithName("SearchFiles")
                 .RequireAuthorization()
                 .Produces<PagedResult<FileListItem>>(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

            // GET /api/files/{id}  -> download a file
            group.MapGet("/{id:guid}", DownloadFileAsync)
                 .WithName("DownloadFile")
                 .RequireAuthorization()
                 .Produces(StatusCodes.Status200OK)
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);

            // DELETE /api/files/{id} -> delete a file
            group.MapDelete("/{id:guid}", DeleteFileAsync)
                 .WithName("DeleteFile")
                 .RequireAuthorization()
                 .Produces(StatusCodes.Status204NoContent)
                 .ProducesProblem(StatusCodes.Status404NotFound)
                 .ProducesProblem(StatusCodes.Status401Unauthorized);
        }

        private static async Task<IResult> UploadFileAsync([FromForm] UploadFileRequest request, [FromServices] IFileService fileService, CancellationToken ct)
        {
            if (request?.File is null || request.File.Length == 0)
                return Results.BadRequest("File is required.");

            await using var stream = request.File.OpenReadStream();

            var uploadResult = await fileService.UploadAsync(
                stream,
                request.File.FileName,
                request.File.ContentType,
                request.OwnerId,
                request.TenantId,
                ct);

            return Results.Extensions.APIResult_Created($"/api/v1/Files/{uploadResult.Id}", uploadResult);
        }

        private static async Task<IResult> DownloadFileAsync(Guid id, [FromServices] IFileService fileService, CancellationToken ct)
        {
            var download = await fileService.DownloadAsync(id, ct);

            if (download is null)
                return Results.Extensions.APIResult_NotFound("File not found");

            return Results.Extensions.APIResult_File(download.Stream, download.ContentType, download.FileName, download.Size);

        }

        private static async Task<IResult> DeleteFileAsync(Guid id, [FromServices] IFileService fileService, CancellationToken ct)
        {
            var deleted = await fileService.DeleteAsync(id, ct);
            if (!deleted)
                return Results.Extensions.APIResult_NotFound("File not found");

            return Results.Extensions.APIResult_NoContent();
        }

        private static async Task<IResult> GetFilesAsync([FromServices] IFileService fileService, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
        {
            var request = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await fileService.GetFilesAsync(request, ct);
            return Results.Ok(result);
        }

        private static async Task<IResult> SearchFilesAsync([FromQuery] string fileName, [FromServices] IFileService fileService, int pageNumber = 1, int pageSize = 10,
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return Results.BadRequest("File name search term is required.");

            var request = new PaginationRequest
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await fileService.SearchFilesAsync(fileName, request, ct);
            return Results.Ok(result);
        }
    }
}
