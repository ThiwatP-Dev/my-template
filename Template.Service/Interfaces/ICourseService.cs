using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface ICourseService
{
    Task<Guid> CreateAsync(CreateCourseDto request, Guid userId);
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task UpdateAsync(Guid id, CreateCourseDto request, Guid userId);
}