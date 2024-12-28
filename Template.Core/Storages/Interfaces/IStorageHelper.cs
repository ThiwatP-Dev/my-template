using Microsoft.AspNetCore.Http;
using Template.Database.Models;

namespace Template.Core.Storages.Interfaces;

public interface IStorageHelper
{
    Task<Resource> UploadAsync(IFormFile file, Func<IFormFile, string> func);
    Task<IEnumerable<Resource>> UploadAsync(IEnumerable<IFormFile> files, Func<IFormFile, string> func);
    string? GetUrl(string? path, int expiryDay = 1);
}