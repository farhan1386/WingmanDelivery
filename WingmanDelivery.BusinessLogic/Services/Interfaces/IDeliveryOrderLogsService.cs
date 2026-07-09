using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IDeliveryOrderLogsService
    {
        Task<IEnumerable<DeliveryLogsModel>> GetDeliveryOrderLogsAsync();
        Task<DeliveryLogsModel> GetDeliveryOrderLogByIdAsync(Guid uid);
        Task<GridDataModel<DeliveryLogsModel>> GetDeliveryOrderLogsForGridAsync(FilterModel filter);
        Task<DeliveryLogsModel> AddDeliveryOrderLogAsync(DeliveryLogsModel log);
        Task<DeliveryLogsModel> UpdateDeliveryOrderLogAsync(DeliveryLogsModel log);
        Task<int> DeleteDeliveryOrderLogAsync(DeliveryLogsModel log);
    }
}
