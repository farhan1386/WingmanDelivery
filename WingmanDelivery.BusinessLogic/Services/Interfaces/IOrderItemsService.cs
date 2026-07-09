using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IOrderItemsService
    {
        Task<IEnumerable<OrderItemModel>> GetItemsByOrderUidAsync(Guid orderUid);
        Task<OrderItemModel> FindItemAsync(Guid uid);
        Task<OrderItemModel> AddItemToOrderAsync(OrderItemModel model);
        Task<int> RemoveItemFromOrderAsync(OrderItemModel model);
    }
}
