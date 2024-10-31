using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface ILearningPathService
{
    Task<Guid> CreateAsync(CreateLearningPathDto request, Guid userId);
    Task<IEnumerable<LearningPathDto>> GetAllAsync();
    Task UpdateAsync(Guid id, CreateLearningPathDto request, Guid userId);
}