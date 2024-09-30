using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IInstituteService
{
    Task<Guid> CreateAsync(CreateInstituteDto request, Guid userId);
    Task<IEnumerable<InstituteDto>> GetAllAsync();
    Task<PagedDto<InstituteDto>> SearchAsync(int page = 1, int pageSize = 25);
    Task<InstituteDto> GetByIdAsync(Guid id);
    Task UpdateAsync(Guid id, CreateInstituteDto request, Guid userId);
    Task DeleteAsync(Guid id);
}