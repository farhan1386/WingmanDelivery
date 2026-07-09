using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<IEnumerable<PaymentModel>> GetPaymentsAsync();
        Task<PaymentModel> FindPaymentAsync(Guid uid);
        Task<IEnumerable<PaymentModel>> GetPaymentsByOrderUidAsync(Guid orderUid);
        Task<GridDataModel<PaymentModel>> GetPaymentsForGridAsync(FilterModel filter);
        Task<PaymentModel> ProcessPaymentAsync(PaymentModel model);
        Task<PaymentModel> RefundPaymentAsync(PaymentModel model);
    }
}
