using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;
using Template.Database.Models.Localizations;
using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;
using Template.Utility.Extensions;

namespace Template.Service.src;

public class InstituteService(IUnitOfWork unitOfWork) : IInstituteService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<Institute> _instituteRepository = unitOfWork.Repository<Institute>();
    private readonly IGenericRepository<InstituteLocalization> _instituteLocalizationRepository = unitOfWork.Repository<InstituteLocalization>();

    public async Task<Guid> CreateAsync(CreateInstituteDto request, Guid userId)
    {
        var defaultLocale = request.Localizations.SingleOrDefault(x => x.Language == LanguageCode.EN);
        var institute = new Institute
        {
            Name = defaultLocale?.Name ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId,
            Localizations = (from localization in request.Localizations
                             select new InstituteLocalization
                             {
                                 Language = localization.Language,
                                 Name = localization.Name
                             })
                            .ToList()
        };

        await _unitOfWork.BeginTransactionAsync();

        await _instituteRepository.CreateAsync(institute);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        return institute.Id;
    }

    public async Task<IEnumerable<InstituteDto>> GetAllAsync(LanguageCode language)
    {
        var institutes = await _instituteRepository.Query(isTracked: false)
                                                   .Include(x => x.Localizations)
                                                   .ToListAsync();

        var response = (from institute in institutes
                        select InstituteMapper.Map(institute, language))
                       .ToList();

        return response;
    }

    public async Task<PagedDto<InstituteDto>> SearchAsync(int page = 1, int pageSize = 25, LanguageCode language = LanguageCode.EN)
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
                     select InstituteMapper.Map(item, language))
                    .ToList()
        };

        return response;
    }

    public async Task<InstituteDto> GetByIdAsync(Guid id, LanguageCode language)
    {
        var institute = await _instituteRepository.Query()
                                                  .Include(x => x.Localizations)
                                                  .SingleOrDefaultAsync(x => x.Id == id);

        if (institute is null)
        {
            throw new CustomException.NotFound("Institute not found");
        }

        var response = InstituteMapper.Map(institute, language);

        return response;
    }

    public async Task UpdateAsync(Guid id, CreateInstituteDto request, Guid userId)
    {
        var institute = await _instituteRepository.Query()
                                                  .Include(x => x.Localizations)
                                                  .SingleOrDefaultAsync(x => x.Id == id);

        if (institute is null)
        {
            throw new CustomException.NotFound("Institute not found");
        }

        await _unitOfWork.BeginTransactionAsync();

        _instituteLocalizationRepository.DeleleRange(institute.Localizations);

        var defaultLocale = request.Localizations.SingleOrDefault(x => x.Language == LanguageCode.EN);
        institute.Name = defaultLocale?.Name ?? string.Empty;
        institute.UpdatedAt = DateTime.UtcNow;
        institute.UpdatedBy = userId;
        institute.Localizations = (from localization in request.Localizations
                                   select new InstituteLocalization
                                   {
                                       Language = localization.Language,
                                       Name = localization.Name
                                   })
                                  .ToList();

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
        IQueryable<Institute> query = _instituteRepository.Query(isTracked: false)
                                                          .Include(x => x.Localizations);

        query = query.OrderBy(x => x.Name);

        return query;
    }
}