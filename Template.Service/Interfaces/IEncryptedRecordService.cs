using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IEncryptedRecordService
{
    Task CreateAsync(CreateEncryptedRecordDto request);
    Task<IEnumerable<EncryptedRecordDto>> GetAllAsync();
}