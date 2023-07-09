using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Org.BouncyCastle.Asn1.Ocsp;


namespace booking_my_doctor.Services.PaymentService
{

    public interface IPaymentService
    {

        Task<ApiResponse> GetPaymentListAsync(int? page = null, int? pageSize = null, int? doctorId = null, bool? status = null, DateTime? date = null);

        Task<ApiResponse> CreatePaymentAsync(Payment payment);

        Task<ApiResponse> DeletePayment(int id);

        Task<ApiResponse> GetPaymentById(int id);
        Task<ApiResponse> CreatePayment(int doctorId);
        Task<ApiResponse> GetPaymentInfo(int doctorId);
        Task<ApiResponse> ReturnPayment(IQueryCollection vnpayData);
    }
}
