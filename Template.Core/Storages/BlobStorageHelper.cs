using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Template.Core.Configs;
using Template.Core.Storages.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;

namespace Template.Core.Storages;

public class BlobStorageHelper : IStorageHelper
{
    private readonly BlobStorageConfiguration _configuration;
    private readonly BlobContainerClient _containerClient;

    public BlobStorageHelper(IOptions<BlobStorageConfiguration> configurationOptions)
    {
        _configuration = configurationOptions.Value;
        _containerClient = new BlobContainerClient(_configuration.ConnectionString,
                                                   _configuration.ContainerName);
    }

    public async Task<Resource> UploadAsync(IFormFile file, Func<IFormFile, string> func)
    {
        var path = func(file);
        var response = new Resource
        {
            Path = path,
            Name = file.FileName,
            Type = GetFileType(file.ContentType),
            Size = file.Length,
            Date = DateTime.UtcNow
        };

        if (response.Type == FileType.VIDEO)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    file.CopyTo(memoryStream);

                    // Create a TagLib file from the memory stream
                    memoryStream.Seek(0, SeekOrigin.Begin); // Reset the stream position
                    var video = TagLib.File.Create(new StreamFileAbstraction(file.FileName, memoryStream, memoryStream));

                    // Get the duration in seconds
                    response.Duration = video.Properties.Duration.TotalSeconds;
                }
            }
            catch { }
        }

        var blobClient = _containerClient.GetBlobClient(path);

        var headers = new BlobHttpHeaders
        {
            ContentType = file.ContentType
        };

        await blobClient.UploadAsync(file.OpenReadStream(), headers);

        return response;
    }

    public async Task<IEnumerable<Resource>> UploadAsync(IEnumerable<IFormFile> files, Func<IFormFile, string> func)
    {
        var response = new List<Resource>();
        foreach (var file in files)
        {
            var resource = await UploadAsync(file, func);

            response.Add(resource);
        }

        return response;
    }

    public string? GetUrl(string? path, int expiryDay = 1)
    {
        if (string.IsNullOrEmpty(path))
        {
            return null;
        }

        var blockClient = _containerClient.GetBlobClient(path);

        if (!blockClient.CanGenerateSasUri)
        {
            return null;
        }

        var uri = blockClient.GenerateSasUri(BlobSasPermissions.Read, DateTime.UtcNow.AddDays(expiryDay));

        return uri.AbsoluteUri.Replace(uri.Query, string.Empty);
    }

    private static FileType GetFileType(string contentType)
    {
        if (contentType.StartsWith("image"))
        {
            return FileType.IMAGE;
        }
        else if (contentType.StartsWith("video"))
        {
            return FileType.VIDEO;
        }

        return FileType.UNDEFINED;
    }
}

public class StreamFileAbstraction(string name, Stream readStream, Stream writeStream) : TagLib.File.IFileAbstraction
{
    public string Name { get; } = name;

    public Stream ReadStream { get; } = readStream;

    public Stream WriteStream { get; } = writeStream;

    public void CloseStream(Stream stream) { }
}