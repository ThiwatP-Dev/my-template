using Microsoft.EntityFrameworkCore;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.Service.src;

public class EncryptedRecordService(IUnitOfWork unitOfWork) : IEncryptedRecordService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IGenericRepository<EncryptedRecord> _encryptedRecordRepository = unitOfWork.Repository<EncryptedRecord>();

    public async Task CreateAsync(CreateEncryptedRecordDto request)
    {
        var model = new EncryptedRecord
        {
            SensitiveData = request.SensitiveData
        };

        await _unitOfWork.BeginTransactionAsync();
        await _encryptedRecordRepository.CreateAsync(model);

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task<IEnumerable<EncryptedRecordDto>> GetAllAsync()
    {
        var records = await _encryptedRecordRepository.Query()
                                                      .ToListAsync();

        var response = (from record in records
                        select EncryptedRecordMapper.Map(record))
                       .ToList();

        return response;
    }
}