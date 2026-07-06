using Microsoft.Data.SqlClient;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;
using WingmanDelivery.Server.BackgroundServices;
using WingmanDelivery.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();
    string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=WingmanDb;Integrated Security=True;Persist Security Info=True;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=True;TrustServerCertificate=True;Command Timeout=60";
    var connection = new SqlConnection(connectionString);
    var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
    string? headerValue = httpContext?.Request.Headers["X-Tenant-Schema"];
    string activeSchema = string.IsNullOrWhiteSpace(headerValue) ? "dbo" : headerValue;

    var contextData = new InvokeDataModel
    {
        userUid = Guid.NewGuid(),
        schema = activeSchema,
        commandTimeout = 30,
        configuration = config
    };

    return new UnitOfWork(connection, contextData);
});

builder.Services.AddScoped<IDeliveryOrderRepository>(provider =>
    provider.GetRequiredService<IUnitOfWork>().Orders);
builder.Services.AddScoped<IDeliveryOrderLogsRepository>(provider =>
    provider.GetRequiredService<IUnitOfWork>().Logs);
builder.Services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
builder.Services.AddScoped<IDeliveryOrderLogsService, DeliveryOrderLogsService>();
builder.Services.AddHostedService<DeliverySimulationWorker>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapHub<DeliveryTrackingHub>("/hubs/tracking");

var api = app.MapGroup("/api");

api.MapPost("/orders", async (CreateOrderDto dto, IDeliveryOrderService orderService) =>
{
    var orderModel = new DeliveryOrderModel
    {
        f_uid = Guid.NewGuid(),
        f_pickup_address = dto.PickupAddress,
        f_status = WingmanDelivery.Models.Enums.OrderStatus.Pending,
        f_total_cost = 15.50m
    };

    var newOrder = await orderService.AddDeliveryOrderAsync(orderModel);

    return Results.Created($"/api/orders/{newOrder.f_uid}", newOrder);
});

app.MapDefaultEndpoints();
app.UseFileServer();
app.Run();
public record CreateOrderDto(string PickupAddress);
