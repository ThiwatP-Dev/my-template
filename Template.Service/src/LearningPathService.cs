using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;

namespace Template.Service.src;

public class LearningPathService(IUnitOfWork unitOfWork) : ILearningPathService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<LearningPath> _learningPathRepository = unitOfWork.Repository<LearningPath>();
    
    public async Task<Guid> CreateAsync(CreateLearningPathDto request, Guid userId)
    {
        var path = new LearningPath
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        if (!string.IsNullOrEmpty(request.Description) 
            || !string.IsNullOrEmpty(request.Remark))
        {
            path.Properties = new LearningPathProperties
            {
                Description = request.Description,
                Remark = request.Remark
            };
        }

        await _unitOfWork.BeginTransactionAsync();

        await _learningPathRepository.CreateAsync(path);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        return path.Id;
    }

    public async Task<IEnumerable<LearningPathDto>> GetAllAsync()
    {
        var paths = await _learningPathRepository.Query(isTracked: false)
                                                 .ToListAsync();

        var response = (from path in paths
                        select LearningPathMapper.Map(path))
                       .ToList();

        return response;
    }

    public async Task UpdateAsync(Guid id, CreateLearningPathDto request, Guid userId)
    {
        var path = await _learningPathRepository.GetByIdAsync(id);

        if (path is null)
        {
            throw new CustomException.NotFound("Path not found");
        }

        await _unitOfWork.BeginTransactionAsync();

        path.Name = request.Name;
        path.UpdatedAt = DateTime.UtcNow;
        path.UpdatedBy = userId;
        path.Properties = null;

        if (!string.IsNullOrEmpty(request.Description) 
            || !string.IsNullOrEmpty(request.Remark))
        {
            path.Properties = new LearningPathProperties
            {
                Description = request.Description,
                Remark = request.Remark
            };
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }
}