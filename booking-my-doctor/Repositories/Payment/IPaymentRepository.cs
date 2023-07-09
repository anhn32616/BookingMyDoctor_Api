using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IPaymentRepository
    {
        Task<PaginationDTO<Payment>> GetPaymentListAsync(int? page = null, int? pageSize = null, int? doctorId = null, bool? status = null, DateTime? date = null);

        Task<bool> CreatePaymentAsync(Payment payment);

        void DeletePayment(Payment payment);

        Task<Payment> GetPaymentById(int id);

        Task<bool> IsSaveChange();

        Task<bool> IsPaymentSuccess(int paymentId);
        Task<bool> CreatePayment(int doctorId);
        Task<PaymentInfo> GetPaymentInfo(int doctorId);
        Task<Payment> GetLastPayment();
        Task<bool> HandlePaymentSuccess(int paymentId, string TranId);
        Task<bool> HandlePaymentFailure(int paymentId);
    }
}
