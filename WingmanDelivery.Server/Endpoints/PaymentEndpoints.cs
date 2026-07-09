using Microsoft.AspNetCore.Mvc;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models;
using WingmanDelivery.Server.Extensions;

namespace WingmanDelivery.Server.Endpoints;

public class PaymentEndpoints : IEndpointModule
{
    public void MapEndpoints(IEndpointRouteBuilder app)
    {
        // Define route group prefix: /api/payments
        var group = app.MapGroup("/api/payments").WithTags("Financial Payments");

        group.MapGet("/", GetPayments);
        group.MapGet("/{uid:guid}", FindPayment);
        group.MapGet("/order/{orderUid:guid}", GetPaymentsByOrder);
        group.MapPost("/grid", GetPaymentsForGrid);
        group.MapPost("/process", ProcessPayment);
        group.MapPost("/refund", RefundPayment);
    }

    private static async Task<IResult> GetPayments(IPaymentService paymentService)
    {
        var payments = await paymentService.GetPaymentsAsync();
        return Results.Ok(payments);
    }

    private static async Task<IResult> FindPayment(Guid uid, IPaymentService paymentService)
    {
        var payment = await paymentService.FindPaymentAsync(uid);
        return payment is not null
            ? Results.Ok(payment)
            : Results.NotFound(new { Message = $"Payment execution log {uid} not found." });
    }

    private static async Task<IResult> GetPaymentsByOrder(Guid orderUid, IPaymentService paymentService)
    {
        var payments = await paymentService.GetPaymentsByOrderUidAsync(orderUid);
        return Results.Ok(payments);
    }

    private static async Task<IResult> GetPaymentsForGrid([FromBody] FilterModel filter, IPaymentService paymentService)
    {
        if (filter is null) return Results.BadRequest("Grid filters cannot be null.");

        var gridData = await paymentService.GetPaymentsForGridAsync(filter);
        return Results.Ok(gridData);
    }

    private static async Task<IResult> ProcessPayment([FromBody] PaymentModel model, IPaymentService paymentService)
    {
        if (model is null) return Results.BadRequest("Payment request payload parameters cannot be null.");

        try
        {
            var result = await paymentService.ProcessPaymentAsync(model);
            return Results.Created($"/api/payments/{result.f_uid}", result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> RefundPayment([FromBody] PaymentModel model, IPaymentService paymentService)
    {
        if (model is null) return Results.BadRequest("Refund alteration parameters cannot be null.");

        try
        {
            var result = await paymentService.RefundPaymentAsync(model);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}