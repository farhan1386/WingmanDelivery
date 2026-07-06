using Microsoft.AspNetCore.SignalR;

namespace WingmanDelivery.Server.Hubs;

public class DeliveryTrackingHub : Hub
{
    public async Task SubscribeToOrder(string orderUid)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, orderUid);
    }
    public async Task UnsubscribeFromOrder(string orderUid)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, orderUid);
    }
}