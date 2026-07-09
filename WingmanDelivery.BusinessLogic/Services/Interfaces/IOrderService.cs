using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderModel>> GetOrdersAsync();
        Task<OrderModel> FindOrderAsync(Guid uid);
        Task<GridDataModel<OrderModel>> GetOrdersForGridAsync(FilterModel filter);
        Task<OrderModel> CreateOrderAsync(OrderModel model);
        Task<OrderModel> UpdateOrderAsync(OrderModel model);
        Task<int> CancelOrderAsync(OrderModel model);
    }
}