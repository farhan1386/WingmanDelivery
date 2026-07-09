using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class OrderItemEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Set the root route path prefix: /api/order-items
        var group = app.MapGroup("/api/order-items").WithTags("Order Line Items");

        group.MapGet("/order/{orderUid:guid}", GetItemsByOrder);
        group.MapGet("/{uid:guid}", FindItem);
        group.MapPost("/", AddItem);
        group.MapPost("/remove", RemoveItem);
    }

    private static async Task<IResult> GetItemsByOrder(Guid orderUid, IOrderItemsService itemService)
    {
        var items = await itemService.GetItemsByOrderUidAsync(orderUid);
        return Results.Ok(items);
    }

    private static async Task<IResult> FindItem(Guid uid, IOrderItemsService itemService)
    {
        var item = await itemService.FindItemAsync(uid);
        return item is not null
            ? Results.Ok(item)
            : Results.NotFound(new { Message = $"Order item row {uid} not found." });
    }

    private static async Task<IResult> AddItem([FromBody] OrderItemModel model, IOrderItemsService itemService)
    {
        if (model is null) return Results.BadRequest("Order item data object cannot be null.");

        try
        {
            var createdItem = await itemService.AddItemToOrderAsync(model);
            return Results.Created($"/api/order-items/{createdItem.f_uid}", createdItem);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> RemoveItem([FromBody] OrderItemModel model, IOrderItemsService itemService)
    {
        if (model is null) return Results.BadRequest("Target removal payload cannot be null.");

        try
        {
            var recordsAffected = await itemService.RemoveItemFromOrderAsync(model);
            return Results.Ok(new { Success = true, RowsRemoved = recordsAffected });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}