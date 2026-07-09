using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Interfaces;

public interface IOrderItemsRepository
{
    Task<IEnumerable<OrderItemModel>> Get();
    Task<OrderItemModel> Find(Guid uid);
    Task<OrderItemModel> Add(OrderItemModel model);
    Task<OrderItemModel> Update(OrderItemModel model);
    Task<int> Remove(OrderItemModel model);
    Task<IEnumerable<OrderItemModel>> GetByOrderUidAsync(Guid orderUid);
}