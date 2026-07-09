using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class OrderItemsService : IOrderItemsService
    {
        private readonly IOrderItemsRepository _orderItemsRepository;
        private readonly IUnitOfWork _unitOfWork;

        public OrderItemsService(
            IOrderItemsRepository orderItemsRepository,
            IUnitOfWork unitOfWork)
        {
            _orderItemsRepository = orderItemsRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<OrderItemModel>> GetItemsByOrderUidAsync(Guid orderUid)
        {
            // Assuming your repository contains an explicit collection lookup for a specific basket
            return await _orderItemsRepository.GetByOrderUidAsync(orderUid);
        }

        public async Task<OrderItemModel> FindItemAsync(Guid uid)
        {
            return await _orderItemsRepository.Find(uid);
        }

        public async Task<OrderItemModel> AddItemToOrderAsync(OrderItemModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var addedItem = await _orderItemsRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return addedItem;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> RemoveItemFromOrderAsync(OrderItemModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var removedCount = await _orderItemsRepository.Remove(model);
                await _unitOfWork.CommitAsync();
                return removedCount;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}