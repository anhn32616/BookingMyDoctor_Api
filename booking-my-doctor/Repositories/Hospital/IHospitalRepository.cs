using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IHospitalRepository
    {
        Task<PaginationDTO<Hospital>> GetHospitals(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<Hospital> GetHospitalById(int id);
        Task<bool> CreateHospital(Hospital hospital);
        Task<bool> UpdateHospital(Hospital hospital);
        Task<bool> DeleteHospital(Hospital hospital);
        Task<bool> IsSaveChanges();
    }
}
