using Microsoft.AspNetCore.Mvc;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class DeliveryOrderLogsEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Define route group prefix: /api/delivery-logs
        var group = app.MapGroup("/api/delivery-logs").WithTags("Delivery Order Logs");

        // 1. GET: /api/delivery-logs
        group.MapGet("/", GetDeliveryOrderLogs);

        // 2. GET: /api/delivery-logs/{uid}
        group.MapGet("/{uid:guid}", GetDeliveryOrderLogById);

        // 3. POST: /api/delivery-logs/grid
        group.MapPost("/grid", GetDeliveryOrderLogsForGrid);

        // 4. POST: /api/delivery-logs
        group.MapPost("/", AddDeliveryOrderLog);

        // 5. PUT: /api/delivery-logs
        group.MapPut("/", UpdateDeliveryOrderLog);

        // 6. POST: /api/delivery-logs/remove
        group.MapPost("/remove", DeleteDeliveryOrderLog);
    }

    private static async Task<IResult> GetDeliveryOrderLogs(IDeliveryOrderLogsService logsService)
    {
        var logs = await logsService.GetDeliveryOrderLogsAsync();
        return Results.Ok(logs);
    }

    private static async Task<IResult> GetDeliveryOrderLogById(Guid uid, IDeliveryOrderLogsService logsService)
    {
        var log = await logsService.GetDeliveryOrderLogByIdAsync(uid);
        return log is not null
            ? Results.Ok(log)
            : Results.NotFound(new { Message = $"Delivery log {uid} not found." });
    }

    private static async Task<IResult> GetDeliveryOrderLogsForGrid([FromBody] FilterModel filter, IDeliveryOrderLogsService logsService)
    {
        if (filter is null) return Results.BadRequest("Filter model cannot be null.");

        var gridData = await logsService.GetDeliveryOrderLogsForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> AddDeliveryOrderLog([FromBody] DeliveryLogsModel log, IDeliveryOrderLogsService logsService)
    {
        if (log is null) return Results.BadRequest("Log model cannot be null.");

        try
        {
            var addedLog = await logsService.AddDeliveryOrderLogAsync(log);
            return Results.Created($"/api/delivery-logs/{addedLog.f_uid}", addedLog);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateDeliveryOrderLog([FromBody] DeliveryLogsModel log, IDeliveryOrderLogsService logsService)
    {
        if (log is null) return Results.BadRequest("Log model cannot be null.");

        try
        {
            var updatedLog = await logsService.UpdateDeliveryOrderLogAsync(log);
            return Results.Ok(updatedLog);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> DeleteDeliveryOrderLog([FromBody] DeliveryLogsModel log, IDeliveryOrderLogsService logsService)
    {
        if (log is null) return Results.BadRequest("Log model cannot be null.");

        try
        {
            var removedCount = await logsService.DeleteDeliveryOrderLogAsync(log);
            return Results.Ok(new { Success = true, RowsRemoved = removedCount });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}