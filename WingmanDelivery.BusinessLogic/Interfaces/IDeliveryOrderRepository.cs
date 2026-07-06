using WingmanDelivery.Models;
using WingmanDelivery.Models.Enums;

namespace WingmanDelivery.BusinessLogic.Interfaces;

public interface IDeliveryOrderRepository
{
    Task<IEnumerable<DeliveryOrderModel>> Get();
    Task<DeliveryOrderModel> Find(Guid uid);
    Task<GridDataModel<DeliveryOrderModel>> GetExtendedForGrid(FilterModel filter);
    Task<DeliveryOrderModel> Add(DeliveryOrderModel model);
    Task<DeliveryOrderModel> Update(DeliveryOrderModel model);
    Task<int> Remove(DeliveryOrderModel model);
    Task<IEnumerable<DeliveryOrderModel>> GetActiveOrdersAsync();
}
