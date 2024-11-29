using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Enums;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;
using Template.Utility.Exceptions;
using Template.Utility.Extensions;

namespace Template.Service.src;

public class UserService(IUnitOfWork unitOfWork) : IUserService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<ApplicationUser> _userRepository = unitOfWork.Repository<ApplicationUser>();
    private readonly IGenericRepository<Learner> _learnerRepository = unitOfWork.Repository<Learner>();
    private readonly IGenericRepository<Lecturer> _lecturerRepository = unitOfWork.Repository<Lecturer>();

    public async Task GenerateSuperAdmin()
    {
        if (await _userRepository.AnyAsync(x => x.Role == Role.SUPER_ADMIN))
        {
            return;
        }

        var hashKey = StringExtension.GenerateRandomString(10);
        var admin = new ApplicationUser(Role.SUPER_ADMIN)
        {
            Username = "super_admin",
            FirstName = "super_admin",
            HashedKey = hashKey,
            HashedPassword = "super_admin".HashHMACSHA256(hashKey)
        };

        await _unitOfWork.BeginTransactionAsync();

        await _userRepository.CreateAsync(admin);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task<Guid> CreateLearnerAsync(CreateLearnerDto request, Guid userId)
    {
        var isUsernameExists = await _userRepository.AnyAsync(x => x.Username.Equals(request.Username));
        if (isUsernameExists)
        {
            throw new CustomException.Conflict("Username already exists");
        }

        var learnerExists = await _learnerRepository.AnyAsync(x => x.Email.Equals(request.Email));
        if (learnerExists)
        {
            throw new CustomException.Conflict("Email already exists");
        }

        var code = await GetLearnerCode();
        var hashkey = StringExtension.GenerateRandomString(10);
        var learner = new Learner
        {
            Username = request.Username,
            HashedKey = hashkey,
            HashedPassword = request.Password.HashHMACSHA256(hashkey),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Code = code,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            ProfileUrl = request.ProfileUrl
        };

        await _unitOfWork.BeginTransactionAsync();

        await _learnerRepository.CreateAsync(learner);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        return learner.Id;
    }

    public async Task<Guid> CreateLecturerAsync(CreateLecturerDto request, Guid userId)
    {
        var isUsernameExists = await _userRepository.AnyAsync(x => x.Username.Equals(request.Username));
        if (isUsernameExists)
        {
            throw new CustomException.Conflict("Username already exists");
        }

        var lecturerExists = await _lecturerRepository.AnyAsync(x => x.Email.Equals(request.Email));
        if (lecturerExists)
        {
            throw new CustomException.Conflict("Email already exists");
        }

        var hashkey = StringExtension.GenerateRandomString(10);
        var lecturer = new Lecturer
        {
            Username = request.Username,
            HashedKey = hashkey,
            HashedPassword = request.Password.HashHMACSHA256(hashkey),
            FirstName = request.FirstName,
            MiddleName = request.MiddleName,
            LastName = request.LastName,
            Email = request.Email
        };

        await _unitOfWork.BeginTransactionAsync();

        await _lecturerRepository.CreateAsync(lecturer);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        return lecturer.Id;
    }

    private async Task<string> GetLearnerCode()
    {
        var count = await _learnerRepository.CountAsync();
        return $"{count:D10}";
    }
}