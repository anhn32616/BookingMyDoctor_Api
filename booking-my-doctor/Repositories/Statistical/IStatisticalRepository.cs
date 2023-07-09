using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IStatisticalRepository
    {
        Task<Statistical> GetStatistical(DateTime startTime, DateTime endTime, int? page = null, int? pageSize = null, int? doctorId = null);
        Task<QuantityStatistics> GetQuantityStatistics();
        Task<DoctorRevenue> GetStatisticsOfDoctor(int id, DateTime startTime, DateTime endTime);
    }
}
