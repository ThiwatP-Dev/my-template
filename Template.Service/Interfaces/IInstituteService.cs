using Template.Database.Enums;
using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IInstituteService
{
    Task<Guid> CreateAsync(CreateInstituteDto request, Guid userId);
    Task<IEnumerable<InstituteDto>> GetAllAsync(LanguageCode language);
    Task<PagedDto<InstituteDto>> SearchAsync(int page = 1, int pageSize = 25, LanguageCode language = LanguageCode.EN);
    Task<InstituteDto> GetByIdAsync(Guid id, LanguageCode language);
    Task UpdateAsync(Guid id, CreateInstituteDto request, Guid userId);
    Task DeleteAsync(Guid id);
}