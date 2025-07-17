using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Template.Core.Configs;
using Template.Core.Storages.Interfaces;
using Template.Database.Models;

namespace Template.Core.Storages;

public class MinioStorageHelper(IOptions<MinioConfiguration> configurationOptions,
                                IMinioClient minioClient) : IStorageHelper
{
    private readonly MinioConfiguration _configuration = configurationOptions.Value;
    private readonly IMinioClient _minioClient = minioClient;

    public async Task<Resource> UploadAsync(IFormFile file, Func<IFormFile, string> func)
    {
        var path = func(file);
        var response = FileTypeHelper.GetResource(file, path);

        await EnsureBucketExists(_minioClient);

        var args = new PutObjectArgs()
            .WithBucket(_configuration.BucketName)
            .WithObject(path)
            .WithStreamData(file.OpenReadStream())
            .WithObjectSize(file.Length)
            .WithContentType(file.ContentType);

        await _minioClient.PutObjectAsync(args);

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

        // CUSTOM CLIENT WITH DIFFERENCE ENDPOINT
        var urlClient = new MinioClient()
                        .WithEndpoint(_configuration.PublicEndpoint)
                        .WithCredentials(_configuration.AccessKey, _configuration.SecretKey)
                        .WithSSL(_configuration.Secure)
                        .Build();

        var args = new PresignedGetObjectArgs()
            .WithBucket(_configuration.BucketName)
            .WithObject(path)
            .WithExpiry(expiryDay * 60 * 24);

        var url = await urlClient.PresignedGetObjectAsync(args);

        return url;
    }

    private async Task EnsureBucketExists(IMinioClient minio)
    {
        var args = new BucketExistsArgs().WithBucket(_configuration.BucketName);
        var found = await minio.BucketExistsAsync(args);

        if (found)
        {
            return;
        }

        await minio.MakeBucketAsync(new MakeBucketArgs().WithBucket(_configuration.BucketName));
    }
}