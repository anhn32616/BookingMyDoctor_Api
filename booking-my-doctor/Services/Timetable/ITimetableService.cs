using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.Timetable;

namespace booking_my_doctor.Services
{
    public interface ITimetableService
    {
        Task<ApiResponse> GetTimetables(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null);
        Task<ApiResponse> GetTimetableById(int id);
        Task<ApiResponse> UpdateTimetable(int id, Timetable timetable);
        Task<ApiResponse> CreateTimetable(TimetableCreateDto timetableCreateDto, int doctorId);
        Task<ApiResponse> DeleteTimetable(int id);
    }
}
