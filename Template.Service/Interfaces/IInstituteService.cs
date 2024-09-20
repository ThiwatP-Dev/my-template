using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IInstituteService
{
    Task CreateAsync(CreateInstituteDto request, Guid userId);
    Task<IEnumerable<InstituteDto>> GetAll();
    Task<InstituteDto> GetById(Guid id);
    Task UpdateAsync(Guid id, CreateInstituteDto request, Guid userId);
    Task DeleteAsync(Guid id);
}