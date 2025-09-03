using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Travelogue.Repository.Data;
using Travelogue.Repository.Entities;
using Travelogue.Service.Commons.SignalR;

namespace Travelogue.Service.Services;

public interface INotificationService
{
    Task SendNotificationAsync(Guid userId, string message);
    Task<List<Announcement>> GetUserNotificationsAsync(Guid userId);
    Task MarkAsReadAsync(Guid notificationId);
    Task<bool> SendNotificationToAllAsync(string message);
    Task<bool> SendNotificationAsync2(Guid userId, string message);
}

public class NotificationService : INotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IUnitOfWork _unitOfWork;

    public NotificationService(IHubContext<NotificationHub> hubContext, IUnitOfWork unitOfWork)
    {
        _hubContext = hubContext;
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> SendNotificationToAllAsync(string message)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync("ReceiveNotification", message, null, true);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> SendNotificationAsync2(Guid userId, string message)
    {
        try
        {
            var notification = new Announcement
            {
                UserId = userId,
                Content = message,
                Title = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AnnouncementRepository.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task SendNotificationAsync(Guid userId, string message)
    {
        try
        {
            var notification = new Announcement
            {
                UserId = userId,
                Content = message,
                Title = message,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.AnnouncementRepository.AddAsync(notification);
            await _unitOfWork.SaveAsync();

            await _hubContext.Clients.User(userId.ToString()).SendAsync("ReceiveNotification", message);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public async Task<List<Announcement>> GetUserNotificationsAsync(Guid userId)
    {
        try
        {
            return await _unitOfWork.AnnouncementRepository
                .ActiveEntities
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error marking notification as read: {ex.Message}");
        }
    }

    public async Task MarkAsReadAsync(Guid notificationId)
    {
        try
        {
            var notification = await _unitOfWork.AnnouncementRepository
                .ActiveEntities
                .FirstOrDefaultAsync(n => n.Id == notificationId);

            if (notification != null)
            {
                notification.IsRead = true;
                _unitOfWork.AnnouncementRepository.Update(notification);
                await _unitOfWork.SaveAsync();
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error marking notification as read: {ex.Message}");
        }
    }
}