using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface IFacultyService
{
    Task CreateAsync(CreateFacultyDto request, Guid userId);
    Task<IEnumerable<FacultyDto>> GetAll();
    Task<FacultyDto> GetById(Guid id);
    Task UpdateAsync(Guid id, CreateFacultyDto request, Guid userId);
    Task DeleteAsync(Guid id);
}