using WingmanDelivery.BusinessLogic.Interfaces;
using WingmanDelivery.BusinessLogic.Services.Interfaces;
using WingmanDelivery.BusinessLogic.UnitOfWork;
using WingmanDelivery.Models;

namespace WingmanDelivery.BusinessLogic.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PaymentService(
            IPaymentRepository paymentRepository,
            IUnitOfWork unitOfWork)
        {
            _paymentRepository = paymentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PaymentModel>> GetPaymentsAsync()
        {
            return await _paymentRepository.Get();
        }

        public async Task<PaymentModel> FindPaymentAsync(Guid uid)
        {
            return await _paymentRepository.Find(uid);
        }

        public async Task<IEnumerable<PaymentModel>> GetPaymentsByOrderUidAsync(Guid orderUid)
        {
            return await _paymentRepository.GetByOrderUidAsync(orderUid);
        }

        public async Task<GridDataModel<PaymentModel>> GetPaymentsForGridAsync(FilterModel filter)
        {
            return await _paymentRepository.GetExtendedForGrid(filter);
        }

        public async Task<PaymentModel> ProcessPaymentAsync(PaymentModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var processedPayment = await _paymentRepository.Add(model);
                await _unitOfWork.CommitAsync();
                return processedPayment;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<PaymentModel> RefundPaymentAsync(PaymentModel model)
        {
            await _unitOfWork.BeginAsync();
            try
            {
                var refundedPayment = await _paymentRepository.Update(model);
                await _unitOfWork.CommitAsync();
                return refundedPayment;
            }
            catch
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }
    }
}