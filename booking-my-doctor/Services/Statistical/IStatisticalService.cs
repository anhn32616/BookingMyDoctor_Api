using booking_my_doctor.DTOs;

namespace booking_my_doctor.Services
{
    public interface IStatisticalService
    {
        Task<ApiResponse> GetStatistical(DateTime startTime, DateTime endTime, int? page = null, int? pageSize = null, int? doctorId = null);
        Task<ApiResponse> GetQuantityStatistics();
        Task<ApiResponse> GetStatisticsOfDoctor(int id, DateTime startTime, DateTime endTime);
    }
}
