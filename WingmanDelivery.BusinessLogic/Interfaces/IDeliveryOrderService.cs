using System;
using System.Collections.Generic;
using System.Text;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Interfaces
{
    public interface IDeliveryOrderService
    {
        Task<IEnumerable<DeliveryOrderModel>> GetDeliveryOrderAsync();
        Task<DeliveryOrderModel> FindDeliveryOrderAsync(Guid uid);
        Task<GridDataModel<DeliveryOrderModel>> GetDeliveryOrdersForGridAsync(FilterModel filter);
        Task<DeliveryOrderModel> AddDeliveryOrderAsync(DeliveryOrderModel model);
        Task<DeliveryOrderModel> UpdateDeliveryOrderAsync(DeliveryOrderModel model);
        Task<int> RemoveDeliveryOrderAsync(DeliveryOrderModel model);
    }
}
