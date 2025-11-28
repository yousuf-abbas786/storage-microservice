using Microsoft.Extensions.Options;
using StorageService.Application.Options;
using StorageService.Application.Services.Interfaces;

namespace StorageService.Infrastructure
{
    public class LocalFileStorageProvider : IFileStorageProvider
    {
        private readonly string _rootPath;

        public LocalFileStorageProvider(IOptions<StorageOptions> options)
        {
            _rootPath = options.Value.RootPath
                        ?? Path.Combine(AppContext.BaseDirectory, "storage");
            Directory.CreateDirectory(_rootPath);
        }

        public async Task<StoreFileResult> StoreAsync(Stream fileStream, string fileName, string? contentType, string container, CancellationToken cancellationToken = default)
        {
            var extension = Path.GetExtension(fileName);
            var fileKey = $"{DateTime.UtcNow:yyyy/MM/dd}/{Guid.NewGuid()}{extension}";

            var containerPath = Path.Combine(_rootPath, container);
            Directory.CreateDirectory(containerPath);

            var fullPath = Path.Combine(containerPath, fileKey.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            await using (var file = File.Create(fullPath))
            {
                await fileStream.CopyToAsync(file, cancellationToken);
            }

            var fileInfo = new FileInfo(fullPath);

            return new StoreFileResult
            {
                FileKey = fileKey,
                Length = fileInfo.Length
            };
        }

        public Task<Stream> GetAsync(string container, string fileKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(_rootPath, container,
                fileKey.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
                throw new FileNotFoundException("File not found", fullPath);

            Stream stream = File.OpenRead(fullPath);
            return Task.FromResult(stream);
        }

        public Task<bool> DeleteAsync(string container, string fileKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(_rootPath, container,
                fileKey.Replace('/', Path.DirectorySeparatorChar));

            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            File.Delete(fullPath);
            return Task.FromResult(true);
        }

        public Task<bool> ExistsAsync(string container, string fileKey, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var fullPath = Path.Combine(_rootPath, container,
                fileKey.Replace('/', Path.DirectorySeparatorChar));
            return Task.FromResult(File.Exists(fullPath));
        }


    }
}
