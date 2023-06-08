using booking_my_doctor.Data.Entities;

namespace booking_my_doctor.Repositories
{
    public interface IRoleRepository
    {
        Task<List<Role>> getAll();
        Task<Role> getRoleByName(string name);
    }
}
