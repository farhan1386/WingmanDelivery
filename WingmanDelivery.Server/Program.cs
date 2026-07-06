using Microsoft.Data.SqlClient;
using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;
using WingmanDelivery.Server.BackgroundServices;
using WingmanDelivery.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. Add standard cloud-native service defaults & Aspire hooks
builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

// 2. Add SignalR services to the DI engine container
builder.Services.AddSignalR();

// 3. Inject Asynchronous SQL Server Unit of Work Pipeline Dependencies
// 3. Inject Asynchronous SQL Server Unit of Work Pipeline Dependencies
builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var config = provider.GetRequiredService<IConfiguration>();

    // 💡 THE FIX: Connects to your exact SQLExpress instance, targets WingmanDb, and uses the 'sa' profile securely
    string connectionString = "Data Source=localhost\\SQLEXPRESS;Initial Catalog=WingmanDb;Integrated Security=True;Persist Security Info=True;Pooling=False;MultipleActiveResultSets=False;Connect Timeout=60;Encrypt=True;TrustServerCertificate=True;Command Timeout=60";

    // Instantiates the raw ADO.NET connection session pointing directly to your local SQLExpress
    var connection = new SqlConnection(connectionString);

    var httpContext = provider.GetService<IHttpContextAccessor>()?.HttpContext;
    string activeSchema = httpContext?.Request.Headers["X-Tenant-Schema"].ToString() ?? "dbo";

    var contextData = new InvokeDataModel
    {
        userUid = Guid.NewGuid(),
        schema = activeSchema,
        commandTimeout = 30,
        configuration = config
    };

    return new UnitOfWork(connection, contextData);
});


// 4. Register the repository interfaces by pulling them straight out of your active Unit of Work
builder.Services.AddScoped<IDeliveryOrderRepository>(provider =>
    provider.GetRequiredService<IUnitOfWork>().Orders);

builder.Services.AddScoped<IDeliveryOrderLogsRepository>(provider =>
    provider.GetRequiredService<IUnitOfWork>().Logs);

// 5. Register your application service layers cleanly
builder.Services.AddScoped<IDeliveryOrderService, DeliveryOrderService>();
builder.Services.AddScoped<IDeliveryOrderLogsService, DeliveryOrderLogsService>();
builder.Services.AddHostedService<DeliverySimulationWorker>();
builder.Services.AddScoped<IDeliveryOrderLogsService, DeliveryOrderLogsService>();


var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 6. Map your continuous Real-Time SignalR communication path
app.MapHub<DeliveryTrackingHub>("/hubs/tracking");

// 7. Expose your business entry HTTP route endpoints
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

// Default cloud-native orchestration routes mapped securely
app.MapDefaultEndpoints();
app.UseFileServer();

app.Run();

public record CreateOrderDto(string PickupAddress);
