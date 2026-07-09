using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WingmanDelivery.BusinessLogic.Interfaces; // Namespace containing your repository contracts
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class DeliveryOrderLogsService : IDeliveryOrderLogsService
    {
        private readonly IDeliveryOrderLogsRepository _deliveryOrderLogsRepository;
        private readonly IUnitOfWork _unitOfWork;

        // Inject BOTH the specific repository and the transactional manager
        public DeliveryOrderLogsService
        (
            IDeliveryOrderLogsRepository deliveryOrderLogsRepository,
            IUnitOfWork unitOfWork
        )
        {
            _deliveryOrderLogsRepository = deliveryOrderLogsRepository ?? throw new ArgumentNullException(nameof(deliveryOrderLogsRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<DeliveryLogsModel>> GetDeliveryOrderLogsAsync()
        {
            // Use the injected independent repository hook directly
            return await _deliveryOrderLogsRepository.Get();
        }

        public async Task<DeliveryLogsModel> GetDeliveryOrderLogByIdAsync(Guid uid)
        {
            return await _deliveryOrderLogsRepository.Find(uid);
        }

        public async Task<GridDataModel<DeliveryLogsModel>> GetDeliveryOrderLogsForGridAsync(FilterModel filter)
        {
            return await _deliveryOrderLogsRepository.GetExtendedForGrid(filter);
        }

        public async Task<DeliveryLogsModel> AddDeliveryOrderLogAsync(DeliveryLogsModel log)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var addedLog = await _deliveryOrderLogsRepository.Add(log);
                await _unitOfWork.CommitAsync();
                return addedLog;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<DeliveryLogsModel> UpdateDeliveryOrderLogAsync(DeliveryLogsModel log)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var updatedLog = await _deliveryOrderLogsRepository.Update(log);
                await _unitOfWork.CommitAsync();
                return updatedLog;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<int> DeleteDeliveryOrderLogAsync(DeliveryLogsModel log)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var deletedCount = await _deliveryOrderLogsRepository.Remove(log);
                await _unitOfWork.CommitAsync();
                return deletedCount;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}