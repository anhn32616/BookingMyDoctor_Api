using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IDoctorRepository
    {
        Task<bool> CreateDoctor(Doctor doctor);
        Task<bool> IsSaveChanges();
        Task<PaginationDTO<Doctor>> GetDoctors(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<Doctor> GetDoctorById(int id);
        Task<bool> UpdateDoctor(Doctor doctor);
        Task<bool> DeleteDoctor(Doctor doctor);
    }
}
