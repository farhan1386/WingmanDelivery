using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork)
        {
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<NotificationModel>> GetNotificationAsync()
        {
            return await _notificationRepository.Get();
        }

        public async Task<NotificationModel> FindNotificationAsync(Guid uid)
        {
            return await _notificationRepository.Find(uid);
        }

        public async Task<IEnumerable<NotificationModel>> GetByUserUidAsync(Guid userUid)
        {
            return await _notificationRepository.GetByUserUidAsync(userUid);
        }

        public async Task<GridDataModel<NotificationModel>> GetExtendedForGridAsync(FilterModel filter)
        {
            return await _notificationRepository.GetExtendedForGrid(filter);
        }

        public async Task<NotificationModel> CreateNotificationAsync(NotificationModel notification)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var createdNotification = await _notificationRepository.Add(notification);
                await _unitOfWork.CommitAsync();
                return createdNotification;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}
