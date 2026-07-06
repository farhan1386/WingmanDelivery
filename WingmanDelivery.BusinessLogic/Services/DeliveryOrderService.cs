using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class DeliveryOrderService : IDeliveryOrderService
    {
        private readonly IDeliveryOrderRepository _deliveryOrderRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryOrderService
        (
            IDeliveryOrderRepository deliveryOrderRepository,
            IUnitOfWork unitOfWork
        )
        {
            _deliveryOrderRepository = deliveryOrderRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DeliveryOrderModel>> GetDeliveryOrderAsync()
        {
            return await _deliveryOrderRepository.Get();
        }

        public async Task<DeliveryOrderModel> FindDeliveryOrderAsync(Guid uid)
        {
            return await _deliveryOrderRepository.Find(uid);
        }

        public async Task<GridDataModel<DeliveryOrderModel>> GetDeliveryOrdersForGridAsync(FilterModel filter)
        {
            return await _deliveryOrderRepository.GetExtendedForGrid(filter);
        }

        public async Task<DeliveryOrderModel> AddDeliveryOrderAsync(DeliveryOrderModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var addedModel = await _deliveryOrderRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return addedModel;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<DeliveryOrderModel> UpdateDeliveryOrderAsync(DeliveryOrderModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedModel = await _deliveryOrderRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return updatedModel;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> RemoveDeliveryOrderAsync(DeliveryOrderModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var removedCount = await _deliveryOrderRepository.Remove(model);
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
