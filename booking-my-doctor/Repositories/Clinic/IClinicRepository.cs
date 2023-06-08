using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IClinicRepository
    {
        Task<PaginationDTO<Clinic>> GetClinics(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<Clinic> GetClinicById(int id);
        Task<bool> CreateClinic(Clinic clinic);
        Task<bool> UpdateClinic(Clinic clinic);
        Task<bool> DeleteClinic(Clinic clinic);
        Task<bool> IsSaveChanges();
    }
}
