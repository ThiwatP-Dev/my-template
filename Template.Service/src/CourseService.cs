using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;

namespace Template.Service.src;

public class CourseService(IUnitOfWork unitOfWork) : ICourseService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<Lecturer> _lecturerRepository = unitOfWork.Repository<Lecturer>();
    private readonly IGenericRepository<Course> _courseRepository = unitOfWork.Repository<Course>();
    private readonly IGenericRepository<CourseLecturer> _courseLecturerRepository = unitOfWork.Repository<CourseLecturer>();

    public async Task<Guid> CreateAsync(CreateCourseDto request, Guid userId)
    {
        if (await _courseRepository.AnyAsync(x => x.Code.Equals(request.Code)))
        {
            throw new CustomException.Conflict("Customer code already exists");
        }
        
        var course = new Course
        {
            Code = request.Code,
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = userId
        };

        if (request.Lecturers is not null && request.Lecturers.Any())
        {
            var lecturers = await _lecturerRepository.Query(x => request.Lecturers.Contains(x.Id))
                                                     .ToListAsync();
            course.Lecturers = lecturers;
        }

        await _unitOfWork.BeginTransactionAsync();

        await _courseRepository.CreateAsync(course);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        return course.Id;
    }
    
    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        var courses = await _courseRepository.Query(isTracked: false)
                                             .Include(x => x.Lecturers)
                                             .ToListAsync();
        
        var response = (from course in courses
                        select CourseMapper.Map(course))
                       .ToList();
        
        return response;
    }

    public async Task UpdateAsync(Guid id, CreateCourseDto request, Guid userId)
    {
        var course = await _courseRepository.GetByIdAsync(id);

        if (course is null)
        {
            throw new CustomException.NotFound("Course not found");
        }

        if (await _courseRepository.AnyAsync(x => x.Id != id
                                                  && x.Code.Equals(request.Code)))
        {
            throw new CustomException.Conflict("Course code already exists");
        }

        await _unitOfWork.BeginTransactionAsync();

        course.Code = request.Code;
        course.Name = request.Name;
        course.UpdatedAt = DateTime.UtcNow;
        course.UpdatedBy = userId;

        var existingCourses = await _courseLecturerRepository.Query(x => x.CourseId == id)
                                                             .ToListAsync();
        
        if (existingCourses is not null && existingCourses.Count > 0)
        {
            _courseLecturerRepository.DeleleRange(existingCourses);
        }

        if (request.Lecturers is not null && request.Lecturers.Any())
        {
            var lecturers = (from lecturerId in request.Lecturers
                             select new CourseLecturer
                             {
                                 CourseId = id,
                                 LecturerId = lecturerId
                             })
                            .ToList();

            await _courseLecturerRepository.CreateRangeAsync(lecturers);
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }
}