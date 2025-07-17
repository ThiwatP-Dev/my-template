using Microsoft.AspNetCore.Http;
using Template.Database.Enums;
using Template.Database.Models;

namespace Template.Core;

public static class FileTypeHelper
{
    public static Resource GetResource(IFormFile file, string path)
    {
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

        return response;
    }
    public static FileType GetFileType(string contentType)
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