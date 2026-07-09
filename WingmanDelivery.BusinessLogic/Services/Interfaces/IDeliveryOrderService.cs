using WingmanDelivery.Models;
using WingmanDelivery.Models.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IDeliveryOrderService
    {
        Task<IEnumerable<DeliveryOrderModel>> GetDeliveryOrderAsync();
        Task<DeliveryOrderModel> FindDeliveryOrderAsync(Guid uid);
        Task<GridDataModel<DeliveryOrderExtendedModel>> GetDeliveryOrdersForGridAsync(FilterModel filter);
        Task<DeliveryOrderModel> AddDeliveryOrderAsync(DeliveryOrderModel model);
        Task<DeliveryOrderModel> UpdateDeliveryOrderAsync(DeliveryOrderModel model);
        Task<int> RemoveDeliveryOrderAsync(DeliveryOrderModel model);
    }
}