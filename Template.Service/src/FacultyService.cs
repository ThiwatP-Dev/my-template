using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.Service.src;

public class FacultyService(IUnitOfWork unitOfWork) : IFacultyService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<Faculty> _facultyRepository = unitOfWork.Repository<Faculty>();

    public async Task CreateAsync(CreateFacultyDto request, Guid userId)
    {
        IEnumerable<Faculty> existingFaculty = await _facultyRepository.QueryAsync(x => x.Code == request.Code);

        if (existingFaculty.Any())
        {
            throw new InvalidOperationException();
        }

        var faculty = new Faculty
        {
            Code = request.Code,
            Name = request.Name,
            Logo = request.Logo,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        await _unitOfWork.BeginTransactionAsync();

        await _facultyRepository.CreateAsync(faculty);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<FacultyDto>> GetAll()
    {
        IEnumerable<Faculty> faculties = await _facultyRepository.QueryAsync();

        var response = (from faculty in faculties
                        orderby faculty.Code
                        select new FacultyDto
                        {
                            Id = faculty.Id,
                            Code = faculty.Code,
                            Name = faculty.Name
                        })
                       .ToList();
        
        return response;
    }

    public async Task<FacultyDto> GetById(Guid id)
    {
        throw new NotImplementedException();
    }
    
    public async Task UpdateAsync(Guid id, CreateFacultyDto request, Guid userId)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}