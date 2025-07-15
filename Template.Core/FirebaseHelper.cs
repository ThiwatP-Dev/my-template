using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Template.Core.Configs;
using Template.Core.Dto.Notifications;
using Template.Database;
using Template.Database.Enums;
using Template.Database.Models;
using AppNotification = Template.Database.Models.Notification;

namespace Template.Core;

public class FirebaseHelper
{
    private readonly DatabaseContext _dbContext;
    private readonly FirebaseMessaging _messaging;

    public FirebaseHelper(FirebaseMessaging messaging,
                          DatabaseContext dbContext)
    {
        _dbContext = dbContext;
        _messaging = messaging;
    }

    public async Task SendNotificationAsync(Guid userId, FirebaseNotificationDto request)
    {
        var notification = new AppNotification
        {
            Title = request.Title,
            Body = request.Body,
            DataPayload = request.Data is null ? null : JsonConvert.SerializeObject(request.Data),
            CreatedAt = DateTime.UtcNow
        };

        var userNotification = new UserNotification
        {
            TargetUserId = userId,
            Notification = notification,
            SentStatus = NotificationSentStatus.PENDING
        };

        await _dbContext.Notifications.AddAsync(notification);
        await _dbContext.UserNotifications.AddAsync(userNotification);
        await _dbContext.SaveChangesAsync();

        var deviceToken = await _dbContext.DeviceTokens.Where(t => t.UserId == userId && t.IsActive)
                                                       .Select(t => t.Token)
                                                       .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(deviceToken))
        {
            userNotification.SentStatus = NotificationSentStatus.FAILED;
            userNotification.ErrorMessage = "No device token found";

            await _dbContext.SaveChangesAsync();
            return;
        }

        var message = new Message
        {
            Token = deviceToken,
            Notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = request.Title,
                Body = request.Body
            },
            Data = request.Data ?? []
        };

        try
        {
            var result = await _messaging.SendAsync(message);

            userNotification.SentStatus = NotificationSentStatus.SENT;
            userNotification.SentAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            userNotification.SentStatus = NotificationSentStatus.FAILED;
            userNotification.ErrorMessage = ex.Message;

            await _dbContext.SaveChangesAsync();
        }
    }
    
    public async Task SendNotificationAsync(List<Guid> userIds, FirebaseNotificationDto request)
    {
        var notification = new AppNotification
        {
            Title = request.Title,
            Body = request.Body,
            DataPayload = request.Data is null ? null : JsonConvert.SerializeObject(request.Data),
            CreatedAt = DateTime.UtcNow
        };

        var userNotifications = new List<UserNotification>();

        foreach (var userId in userIds)
        {
            var userNotification = new UserNotification
            {
                TargetUserId = userId,
                Notification = notification,
                SentStatus = NotificationSentStatus.PENDING
            };

            userNotifications.Add(userNotification);
        }
        
        await _dbContext.Notifications.AddAsync(notification);
        await _dbContext.UserNotifications.AddRangeAsync(userNotifications);
        await _dbContext.SaveChangesAsync();

        // Get all device tokens for these users
        var deviceTokens = await _dbContext.DeviceTokens.Where(t => userIds.Contains(t.UserId) && t.IsActive)
                                                        .Select(t => new { t.UserId, t.Token })
                                                        .ToListAsync();

        var tokensByUser = deviceTokens.ToDictionary(t => t.UserId, t => t.Token);

        foreach (var userNotification in userNotifications)
        {
            var userId = userNotification.TargetUserId;

            if (!tokensByUser.TryGetValue(userId, out var deviceToken) || string.IsNullOrWhiteSpace(deviceToken))
            {
                userNotification.SentStatus = NotificationSentStatus.FAILED;
                userNotification.ErrorMessage = "No device token found";
                continue;
            }

            var message = new Message
            {
                Token = deviceToken,
                Notification = new FirebaseAdmin.Messaging.Notification
                {
                    Title = request.Title,
                    Body = request.Body
                },
                Data = request.Data ?? []
            };

            try
            {
                var result = await _messaging.SendAsync(message);
                userNotification.SentStatus = NotificationSentStatus.SENT;
                userNotification.SentAt = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                userNotification.SentStatus = NotificationSentStatus.FAILED;
                userNotification.ErrorMessage = ex.Message;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
}