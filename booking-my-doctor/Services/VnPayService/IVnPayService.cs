using booking_my_doctor.DTOs;

namespace booking_my_doctor.Services.VnPayService
{

    public interface IVnPayService
    {

        Task<ApiResponse> CreateUrlPayment(int paymentId, double totalPrice);

        Task<ApiResponse> ReturnPayment(IQueryCollection vnpayData);
    }
}
