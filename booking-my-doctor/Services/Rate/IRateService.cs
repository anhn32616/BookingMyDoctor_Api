using booking_my_doctor.DTOs;

namespace booking_my_doctor.Services
{
    public interface IRateService
    {
        Task<ApiResponse> GetRates(int? doctorId = null);
        Task<ApiResponse> GetRateByAppointmentId(int appointmentId);
    }
}
