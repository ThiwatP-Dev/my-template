using Template.Service.Dto;

namespace Template.Service.Interfaces;

public interface INotificationService
{
    Task UpdateDeviceTokenAsync(CreateDeviceTokenDto request, Guid userId);
    Task SendAsync();
}