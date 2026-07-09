using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork)
        {
            _orderRepository = orderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderModel>> GetOrdersAsync()
        {
            return await _orderRepository.Get();
        }

        public async Task<OrderModel> FindOrderAsync(Guid uid)
        {
            return await _orderRepository.Find(uid);
        }

        public async Task<GridDataModel<OrderModel>> GetOrdersForGridAsync(FilterModel filter)
        {
            return await _orderRepository.GetExtendedForGrid(filter);
        }

        public async Task<OrderModel> CreateOrderAsync(OrderModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var addedOrder = await _orderRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return addedOrder;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<OrderModel> UpdateOrderAsync(OrderModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedOrder = await _orderRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return updatedOrder;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> CancelOrderAsync(OrderModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                // Soft deletes or status modifications are handled safely through repository update rules
                var rowsAffected = await _orderRepository.Remove(model);
                await _unitOfWork.CommitAsync();
                return rowsAffected;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}