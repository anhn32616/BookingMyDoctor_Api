using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface ISpeciatlyRepository
    {
        Task<PaginationDTO<Speciatly>> GetSpeciatlies(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id");
        Task<Speciatly> GetSpeciatlyById(int id);
        Task<bool> CreateSpeciatly(Speciatly speciatly);
        Task<bool> UpdateSpeciatly(Speciatly speciatly);
        Task<bool> DeleteSpeciatly(Speciatly speciatly);
        Task<bool> IsSaveChanges();
    }
}
