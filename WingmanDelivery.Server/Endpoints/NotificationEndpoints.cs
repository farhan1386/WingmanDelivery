using Microsoft.AspNetCore.Mvc;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class NotificationEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Define route group prefix matching frontend expectations
        var group = app.MapGroup("/api/notifications").WithTags("System Notifications");

        // 1. GET: /api/notifications
        group.MapGet("/", GetNotifications);

        // 2. GET: /api/notifications/{uid}
        group.MapGet("/{uid:guid}", FindNotification);

        // 3. GET: /api/notifications/user/{userUid:guid}
        group.MapGet("/user/{userUid:guid}", GetNotificationsByUser);

        // 4. POST: /api/notifications/grid
        group.MapPost("/grid", GetNotificationsForGrid);

        // 5. POST: /api/notifications/send
        group.MapPost("/send", SendNotification);
    }

    private static async Task<IResult> GetNotifications(INotificationService notificationService)
    {
        // Aligned to: public async Task<IEnumerable<NotificationModel>> GetNotificationAsync()
        var notifications = await notificationService.GetNotificationAsync();
        return Results.Ok(notifications);
    }

    private static async Task<IResult> FindNotification(Guid uid, INotificationService notificationService)
    {
        // Aligned to: public async Task<NotificationModel> FindNotificationAsync(Guid uid)
        var notification = await notificationService.FindNotificationAsync(uid);
        return notification is not null
            ? Results.Ok(notification)
            : Results.NotFound(new { Message = $"Notification registry entry {uid} not found." });
    }

    private static async Task<IResult> GetNotificationsByUser(Guid userUid, INotificationService notificationService)
    {
        // Aligned to: public async Task<IEnumerable<NotificationModel>> GetByUserUidAsync(Guid userUid)
        var notifications = await notificationService.GetByUserUidAsync(userUid);
        return Results.Ok(notifications);
    }

    private static async Task<IResult> GetNotificationsForGrid([FromBody] FilterModel filter, INotificationService notificationService)
    {
        if (filter is null) return Results.BadRequest("Grid telemetry bounds cannot be null.");

        // Aligned to: public async Task<GridDataModel<NotificationModel>> GetExtendedForGridAsync(FilterModel filter)
        var gridData = await notificationService.GetExtendedForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> SendNotification([FromBody] NotificationModel model, INotificationService notificationService)
    {
        if (model is null) return Results.BadRequest("Notification payload template cannot be null.");

        try
        {
            // Aligned to: public async Task<NotificationModel> CreateNotificationAsync(NotificationModel notification)
            var result = await notificationService.CreateNotificationAsync(model);
            return Results.Created($"/api/notifications/{result.f_uid}", result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}