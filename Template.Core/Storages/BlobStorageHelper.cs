using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Template.Core.Configs;
using Template.Core.Storages.Interfaces;
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
        var response = FileTypeHelper.GetResource(file, path);

        var blobClient = _containerClient.GetBlobClient(path);

        await blobClient.UploadAsync(file.OpenReadStream(), true);

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

    public async Task<string?> GetUrlAsync(string? path, int expiryDay = 1)
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

        return await Task.FromResult(uri.AbsoluteUri.Replace(uri.Query, string.Empty));
    }
}