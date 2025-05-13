using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Template.Service.Dto;

public class FileRequestDto
{
    [FromForm(Name = "file")]
    public required IFormFile File { get; set; }
}