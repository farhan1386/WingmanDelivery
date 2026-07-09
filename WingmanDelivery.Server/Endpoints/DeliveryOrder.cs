using Microsoft.AspNetCore.Mvc;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class DeliveryOrderEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Define route group prefix: /api/delivery
        var group = app.MapGroup("/api/delivery").WithTags("Delivery Orders");

        // 1. GET: /api/delivery
        group.MapGet("/", GetDeliveryOrders);

        // 2. GET: /api/delivery/{uid}
        group.MapGet("/{uid:guid}", FindDeliveryOrder);

        // 3. POST: /api/delivery/grid
        group.MapPost("/grid", GetDeliveryOrdersForGrid);

        // 4. POST: /api/delivery
        group.MapPost("/", AddDeliveryOrder);

        // 5. PUT: /api/delivery
        group.MapPut("/", UpdateDeliveryOrder);

        // 6. POST: /api/delivery/remove (or MapDelete depending on client requirements)
        group.MapPost("/remove", RemoveDeliveryOrder);
    }

    private static async Task<IResult> GetDeliveryOrders(IDeliveryOrderService deliveryOrderService)
    {
        var orders = await deliveryOrderService.GetDeliveryOrderAsync();
        return Results.Ok(orders);
    }

    private static async Task<IResult> FindDeliveryOrder(Guid uid, IDeliveryOrderService deliveryOrderService)
    {
        var order = await deliveryOrderService.FindDeliveryOrderAsync(uid);
        return order is not null
            ? Results.Ok(order)
            : Results.NotFound(new { Message = $"Delivery order {uid} not found." });
    }

    private static async Task<IResult> GetDeliveryOrdersForGrid([FromBody] FilterModel filter, IDeliveryOrderService deliveryOrderService)
    {
        if (filter is null) return Results.BadRequest("Filter model cannot be null.");

        var gridData = await deliveryOrderService.GetDeliveryOrdersForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> AddDeliveryOrder([FromBody] DeliveryOrderModel model, IDeliveryOrderService deliveryOrderService)
    {
        if (model is null) return Results.BadRequest("Order model cannot be null.");

        try
        {
            var addedOrder = await deliveryOrderService.AddDeliveryOrderAsync(model);
            return Results.Created($"/api/delivery/{addedOrder.f_uid}", addedOrder);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateDeliveryOrder([FromBody] DeliveryOrderModel model, IDeliveryOrderService deliveryOrderService)
    {
        if (model is null) return Results.BadRequest("Order model cannot be null.");

        try
        {
            var updatedOrder = await deliveryOrderService.UpdateDeliveryOrderAsync(model);
            return Results.Ok(updatedOrder);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> RemoveDeliveryOrder([FromBody] DeliveryOrderModel model, IDeliveryOrderService deliveryOrderService)
    {
        if (model is null) return Results.BadRequest("Order model cannot be null.");

        try
        {
            var removedCount = await deliveryOrderService.RemoveDeliveryOrderAsync(model);
            return Results.Ok(new { Success = true, RowsRemoved = removedCount });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}