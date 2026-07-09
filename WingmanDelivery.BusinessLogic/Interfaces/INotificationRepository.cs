using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Interfaces;

public interface INotificationRepository
{
    Task<IEnumerable<NotificationModel>> Get();
    Task<NotificationModel> Find(Guid uid);
    Task<GridDataModel<NotificationModel>> GetExtendedForGrid(FilterModel filter);
    Task<NotificationModel> Add(NotificationModel model);
    Task<NotificationModel> Update(NotificationModel model);
    Task<int> Remove(NotificationModel model);
    Task<IEnumerable<NotificationModel>> GetByUserUidAsync(Guid userUid);
}