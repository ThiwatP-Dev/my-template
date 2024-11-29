using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;
using Template.Utility.Extensions;

namespace Template.Service.src;

public class InstituteService(IUnitOfWork unitOfWork) : IInstituteService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<Institute> _instituteRepository = unitOfWork.Repository<Institute>();

    public async Task<Guid> CreateAsync(CreateInstituteDto request, Guid userId)
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

        return institute.Id;
    }

    public async Task<IEnumerable<InstituteDto>> GetAllAsync()
    {
        var institutes = await _instituteRepository.Query(isTracked: false)
                                                   .ToListAsync();

        var response = (from institute in institutes
                        select InstituteMapper.Map(institute))
                       .ToList();

        return response;
    }

    public async Task<PagedDto<InstituteDto>> SearchAsync(int page = 1, int pageSize = 25)
    {
        var query = GenerateQuery();

        var pagedInstitute = await query.GetPagedAsync(page, pageSize);

        var response = new PagedDto<InstituteDto>
        {
            Page = page,
            PageSize = pageSize,
            TotalPage = pagedInstitute.TotalPage,
            TotalItem = pagedInstitute.TotalItem,
            Items = (from item in pagedInstitute.Items
                     select InstituteMapper.Map(item))
                    .ToList()
        };

        return response;
    }

    public async Task<InstituteDto> GetByIdAsync(Guid id)
    {
        var institute = await _instituteRepository.GetByIdAsync(id);

        if (institute is null)
        {
            throw new CustomException.NotFound("Institute not found");
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
            throw new CustomException.NotFound("Institute not found");
        }

        await _unitOfWork.BeginTransactionAsync();

        institute.Name = request.Name;
        institute.UpdatedAt = DateTime.UtcNow;
        institute.UpdatedBy = userId;

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

    private IQueryable<Institute> GenerateQuery()
    {
        var query = _instituteRepository.Query(isTracked: false);

        query = query.OrderBy(x => x.Name);

        return query;
    }
}