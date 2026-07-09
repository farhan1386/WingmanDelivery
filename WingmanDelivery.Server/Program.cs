using Scalar.AspNetCore;
using WingmanDelivery.Server.Extensions;
using WingmanDelivery.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddUnitOfWork(builder.Configuration);
builder.Services.AddRepositories();
builder.Services.AddApplicationServices();
var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseFileServer();
app.MapHub<DeliveryTrackingHub>("/hubs/tracking");
app.MapAllEndpointModules();
app.MapDefaultEndpoints();
app.Run();
