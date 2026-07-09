using WingmanDelivery.Models;
namespace WingmanDelivery.BusinessLogic.Interfaces;

public interface IPaymentRepository
{
    Task<IEnumerable<PaymentModel>> Get();
    Task<PaymentModel> Find(Guid uid);
    Task<GridDataModel<PaymentModel>> GetExtendedForGrid(FilterModel filter);
    Task<PaymentModel> Add(PaymentModel model);
    Task<PaymentModel> Update(PaymentModel model);
    Task<int> Remove(PaymentModel model);
    Task<IEnumerable<PaymentModel>> GetByOrderUidAsync(Guid orderUid);
}