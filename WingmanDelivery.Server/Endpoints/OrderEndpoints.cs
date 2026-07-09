using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class OrderEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Set the base routing endpoint path prefix: /api/orders
        var group = app.MapGroup("/api/orders").WithTags("Core Base Orders");

        group.MapGet("/", GetOrders);
        group.MapGet("/{uid:guid}", FindOrder);
        group.MapPost("/grid", GetOrdersForGrid);
        group.MapPost("/", CreateOrder);
        group.MapPut("/", UpdateOrder);
        group.MapPost("/cancel", CancelOrder);
    }

    private static async Task<IResult> GetOrders(IOrderService orderService)
    {
        var orders = await orderService.GetOrdersAsync();
        return Results.Ok(orders);
    }

    private static async Task<IResult> FindOrder(Guid uid, IOrderService orderService)
    {
        var order = await orderService.FindOrderAsync(uid);
        return order is not null
            ? Results.Ok(order)
            : Results.NotFound(new { Message = $"Order registry entry {uid} not found." });
    }

    private static async Task<IResult> GetOrdersForGrid([FromBody] FilterModel filter, IOrderService orderService)
    {
        if (filter is null) return Results.BadRequest("Grid tracking parameters cannot be null.");

        var gridData = await orderService.GetOrdersForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> CreateOrder([FromBody] OrderModel model, IOrderService orderService)
    {
        if (model is null) return Results.BadRequest("Order creation request payload cannot be null.");

        try
        {
            var createdOrder = await orderService.CreateOrderAsync(model);
            return Results.Created($"/api/orders/{createdOrder.f_uid}", createdOrder);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateOrder([FromBody] OrderModel model, IOrderService orderService)
    {
        if (model is null) return Results.BadRequest("Modification request payload cannot be null.");

        try
        {
            var updatedOrder = await orderService.UpdateOrderAsync(model);
            return Results.Ok(updatedOrder);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> CancelOrder([FromBody] OrderModel model, IOrderService orderService)
    {
        if (model is null) return Results.BadRequest("Target cancellation model parameters cannot be null.");

        try
        {
            var resultsCount = await orderService.CancelOrderAsync(model);
            return Results.Ok(new { Success = true, RowsAffected = resultsCount });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}
