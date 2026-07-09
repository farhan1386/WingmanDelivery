using Microsoft.AspNetCore.SignalR;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.Models.Enums;
using WingmanDelivery.Server.Hubs;

namespace WingmanDelivery.Server.BackgroundServices;

public class DeliverySimulationWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IHubContext<DeliveryTrackingHub> _hubContext;
    private readonly ILogger<DeliverySimulationWorker> _logger;

    public DeliverySimulationWorker(
        IServiceProvider serviceProvider,
        IHubContext<DeliveryTrackingHub> hubContext,
        ILogger<DeliverySimulationWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _hubContext = hubContext;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Wingman Driver Simulator Started.");

        // Fake coordinate tracking database
        var routeSimulations = new System.Collections.Concurrent.ConcurrentDictionary<Guid, double>();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // Background tasks run globally, so they create their own Scoped Unit of Work boundaries
                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IDeliveryOrderService>();

                // 1. Get all ongoing orders
                var activeOrders = await orderService.GetDeliveryOrdersForGridAsync(new Models.FilterModel { Take = 100 });

                foreach (var order in activeOrders.Items)
                {
                    if (order.f_status == OrderStatus.Pending)
                    {
                        _logger.LogInformation($"Driver dispatched for order {order.f_uid}");
                        order.f_status = OrderStatus.DriverAssigned;
                        await orderService.UpdateDeliveryOrderAsync(order);

                        // Push status update via SignalR Hub instantly to the browser group channel
                        await _hubContext.Clients.Group(order.f_uid.ToString()).SendAsync("StatusUpdated", "DriverAssigned");
                    }
                    else if (order.f_status == OrderStatus.DriverAssigned)
                    {
                        order.f_status = OrderStatus.EnRoute;
                        await orderService.UpdateDeliveryOrderAsync(order);
                        await _hubContext.Clients.Group(order.f_uid.ToString()).SendAsync("StatusUpdated", "EnRoute");
                    }
                    else if (order.f_status == OrderStatus.EnRoute)
                    {
                        // 2. Simulate moving coordinates (driving across town)
                        routeSimulations.AddOrUpdate(order.f_uid, 0.01, (key, currentProgress) => currentProgress + 0.01);
                        double progress = routeSimulations[order.f_uid];

                        // Simulating a moving coordinate pair near a city hub center point
                        double currentLat = 40.7128 + (progress * 0.1);
                        double currentLng = -74.0060 + (progress * 0.1);

                        // Broadcast raw vehicle coordinates directly down to our React app hook listeners
                        await _hubContext.Clients.Group(order.f_uid.ToString()).SendAsync("LocationUpdated", currentLat, currentLng);

                        if (progress >= 0.05) // Arrived at destination boundary markers
                        {
                            _logger.LogInformation($"Order {order.f_uid} has been safely delivered!");
                            order.f_status = OrderStatus.Delivered;
                            await orderService.UpdateDeliveryOrderAsync(order);
                            await _hubContext.Clients.Group(order.f_uid.ToString()).SendAsync("StatusUpdated", "Delivered");
                            routeSimulations.TryRemove(order.f_uid, out _);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing driver simulations.");
            }

            // Tick simulation interval cadence cycle every 3 seconds
            await Task.Delay(3000, stoppingToken);
        }
    }
}