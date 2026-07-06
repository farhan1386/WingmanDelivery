using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class DeliveryOrderLogsService : IDeliveryOrderLogsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryOrderLogsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<DeliveryLogsModel>> GetDeliveryOrderLogsAsync()
        {
            return await _unitOfWork.Logs.Get();
        }

        public async Task<DeliveryLogsModel> GetDeliveryOrderLogByIdAsync(Guid uid)
        {
            return await _unitOfWork.Logs.Find(uid);
        }

        public async Task<GridDataModel<DeliveryLogsModel>> GetDeliveryOrderLogsForGridAsync(FilterModel filter)
        {
            return await _unitOfWork.Logs.GetExtendedForGrid(filter);
        }

        public async Task<DeliveryLogsModel> AddDeliveryOrderLogAsync(DeliveryLogsModel log)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var addedLog = await _unitOfWork.Logs.Add(log);
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
                var updatedLog = await _unitOfWork.Logs.Update(log);
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
                var deletedCount = await _unitOfWork.Logs.Remove(log);
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