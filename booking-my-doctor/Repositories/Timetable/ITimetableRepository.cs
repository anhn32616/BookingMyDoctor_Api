using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface ITimetableRepository
    {
        Task<PaginationDTO<Timetable>> GetTimetables(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null);
        Task<Timetable> GetTimetableById(int id);
        Task<bool> CreateTimetable(Timetable Timetable);
        Task<bool> UpdateTimetable(Timetable Timetable);
        Task<bool> DeleteTimetable(Timetable Timetable);
        Task<bool> IsSaveChanges();
    }
}
