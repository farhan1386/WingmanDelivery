using WingmanDelivery.Models;
using WingmanDelivery.Models.Enums;

namespace WingmanDelivery.BusinessLogic.Interfaces;

public interface IOrderRepository
{
    Task<IEnumerable<OrderModel>> Get();
    Task<OrderModel> Find(Guid uid);
    Task<GridDataModel<OrderModel>> GetExtendedForGrid(FilterModel filter);
    Task<OrderModel> Add(OrderModel model);
    Task<OrderModel> Update(OrderModel model);
    Task<int> Remove(OrderModel model);
    Task<IEnumerable<OrderModel>> GetActiveOrdersAsync();
}
