using Microsoft.EntityFrameworkCore;
using Template.Core;
using Template.Core.Dto.Notifications;
using Template.Core.Repositories.Interfaces;
using Template.Core.UnitOfWorks.Interfaces;
using Template.Database.Models;
using Template.Service.Dto;
using Template.Service.Interfaces;

namespace Template.Service.src;

public class NotificationService(IUnitOfWork unitOfWork,
                                 FirebaseHelper firebaseHelper) : INotificationService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly FirebaseHelper _firebaseHelper = firebaseHelper;
    private readonly IGenericRepository<DeviceToken> _deviceTokenRepository = unitOfWork.Repository<DeviceToken>();

    public async Task UpdateDeviceTokenAsync(CreateDeviceTokenDto request, Guid userId)
    {
        var existingToken = await _deviceTokenRepository.SingleOrDefaultAsync(x => x.Token.Equals(request.Token));

        await _unitOfWork.BeginTransactionAsync();

        if (existingToken is null)
        {
            var oldTokens = await _deviceTokenRepository.Query(t => t.UserId == userId
                                                                    && t.Platform == request.Platform
                                                                    && t.IsActive)
                                                        .ToListAsync();

            oldTokens.ForEach(x => x.IsActive = false);

            var newDeviceToken = new DeviceToken
            {
                UserId = userId,
                Token = request.Token,
                Platform = request.Platform,
                IsActive = true
            };

            await _deviceTokenRepository.CreateAsync(newDeviceToken);
        }
        else
        {
            existingToken.UserId = userId;
            existingToken.IsActive = true;
            existingToken.Platform = request.Platform;
            existingToken.CreatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();
    }

    public async Task SendAsync()
    {
        var request = new FirebaseNotificationDto
        {
            Title = "this is title",
            Body = "ihis is body",
            Data = new Dictionary<string, string>
            {
                { "key1", "value1" }
            }
        };

        await _firebaseHelper.SendNotificationAsync(Guid.NewGuid(), request);
    }
}