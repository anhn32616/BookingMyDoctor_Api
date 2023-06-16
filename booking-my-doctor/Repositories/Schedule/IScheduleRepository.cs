using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IScheduleRepository
    {
        Task<PaginationDTO<ScheduleView>> GetSchedules(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null, string? status = null, DateTime? date = null, string? sortColumn = "StartTime");
        Task<Schedule> GetScheduleById(int id);
        Task<bool> CreateSchedule(Schedule Schedule);
        Task<bool> UpdateSchedule(Schedule Schedule);
        Task<bool> DeleteSchedule(Schedule Schedule);
        Task<bool> IsSaveChanges();
        Task<bool> UpdateStatusSchedule(Schedule schedule);
        Task<bool> UpdateScheduleExpired();

    }
}
