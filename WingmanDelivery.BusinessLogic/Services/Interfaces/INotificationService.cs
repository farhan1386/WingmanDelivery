using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<NotificationModel>> GetNotificationAsync();
        Task<NotificationModel> FindNotificationAsync(Guid uid);
        Task<IEnumerable<NotificationModel>> GetByUserUidAsync(Guid userUid);
        Task<GridDataModel<NotificationModel>> GetExtendedForGridAsync(FilterModel filter);
        Task<NotificationModel> CreateNotificationAsync(NotificationModel notification);
    }
}