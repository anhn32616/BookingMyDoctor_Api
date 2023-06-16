using booking_my_doctor.DTOs;


namespace booking_my_doctor.Services
{
    public interface IScheduleService
    {
        Task<ApiResponse> GetSchedules(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null, string? status = null, DateTime? date = null, string? sortColumn = "StartTime");
        Task<ApiResponse> GetScheduleById(int id);
        Task<ApiResponse> UpdateSchedule(int id, ScheduleDto ScheduleDto);  
        Task<ApiResponse> CreateSchedule(ScheduleCreateDto scheduleCreateDto);
        Task<ApiResponse> DeleteSchedule(int id);
        Task<ApiResponse> ChangeStatusScheduleToTrue(int id);
    }
}
