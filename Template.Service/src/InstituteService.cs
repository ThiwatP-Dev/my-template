using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.Service.src;

public class InstitudeService(IUnitOfWork unitOfWork) : IInstituteService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<Institute> _instituteRepository = unitOfWork.Repository<Institute>();

    public async Task CreateAsync(CreateInstituteDto request, Guid userId)
    {
        var institute = new Institute
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        await _unitOfWork.BeginTransactionAsync();

        await _instituteRepository.CreateAsync(institute);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<InstituteDto>> GetAll()
    {
        var institutes = await _instituteRepository.Query()
                                                   .ToListAsync();
        
        var response = (from institute in institutes
                        select new InstituteDto
                        {
                            Id = institute.Id,
                            Name = institute.Name
                        })
                       .ToList();
        
        return response;
    }

    public async Task<InstituteDto> GetById(Guid id)
    {
        var institute = await _instituteRepository.GetByIdAsync(id);

        if (institute is null)
        {
            throw new KeyNotFoundException();
        }

        var response = new InstituteDto
        {
            Id = institute.Id,
            Name = institute.Name
        };

        return response;
    }
    
    public async Task UpdateAsync(Guid id, CreateInstituteDto request, Guid userId)
    {
        var institute = await _instituteRepository.GetByIdAsync(id);

        if (institute is null)
        {
            throw new KeyNotFoundException();
        }

        institute.Name = request.Name;
        institute.UpdatedAt = DateTime.UtcNow;
        institute.UpdatedBy = userId;

        await _unitOfWork.BeginTransactionAsync();

        _instituteRepository.Update(institute);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var institute = await _instituteRepository.GetByIdAsync(id);

        if (institute is null)
        {
            return;
        }
        
        await _unitOfWork.BeginTransactionAsync();

        _instituteRepository.Delete(institute);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }
}