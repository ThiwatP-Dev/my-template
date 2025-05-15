using Newtonsoft.Json;
using Template.Database.Models;

namespace Template.Service.Dto;

public class CreateEncryptedRecordDto
{
    [JsonProperty("sensitiveData")]
    public required string SensitiveData { get; set; }
}

public class EncryptedRecordDto : CreateEncryptedRecordDto
{
    [JsonProperty("id")]
    public Guid Id { get; set; }
}

public static class EncryptedRecordMapper
{
    public static EncryptedRecordDto Map(EncryptedRecord model)
    {
        var response = new EncryptedRecordDto
        {
            Id = model.Id,
            SensitiveData = model.SensitiveData
        };

        return response;
    }
}