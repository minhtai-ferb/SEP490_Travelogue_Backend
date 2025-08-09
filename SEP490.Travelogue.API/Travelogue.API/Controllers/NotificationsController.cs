using Microsoft.AspNetCore.Mvc;
using Travelogue.Repository.Bases.Responses;
using Travelogue.Service.Commons.BaseResponses;
using Travelogue.Service.Services;

namespace Travelogue.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("send")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
    {
        var result = await _notificationService.SendNotificationAsync2(request.UserId, request.Message);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "notification")
        ));
    }

    [HttpPost("send-all")]
    public async Task<IActionResult> SendNotificationToAll([FromBody] NotificationAllRequest request)
    {
        var result = await _notificationService.SendNotificationToAllAsync(request.Message);
        return Ok(ResponseModel<object>.OkResponseModel(
            data: result,
            message: ResponseMessageHelper.FormatMessage(ResponseMessages.CREATE_SUCCESS, "notification")
        ));
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(Guid userId)
    {
        var notifications = await _notificationService.GetUserNotificationsAsync(userId);
        return Ok(notifications);
    }

    [HttpPut("{notificationId}/read")]
    public async Task<IActionResult> MarkAsRead(Guid notificationId)
    {
        await _notificationService.MarkAsReadAsync(notificationId);
        return Ok();
    }
}

public class NotificationRequest
{
    public Guid UserId { get; set; }
    public string Message { get; set; }
}

public class NotificationAllRequest
{
    public string Message { get; set; }
}