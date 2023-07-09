using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Repositories
{
    public interface IUserRepository
    {
        Task<PaginationDTO<User>> GetUsers(int? page, int? pageSize, string? name, string sortColumn, string? roleName);
        Task<User> GetUserById(int id);
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> IsEmailAlreadyExists(string email);
        Task<bool> DeleteUser(User user);
        Task<User> GetUserByEmail(string email);
        Task<bool> VerifiedEmail(string token);
        Task<bool> IsSaveChanges();
        Task<int> GetLastUserID();
        Task<bool> OpenCloseUser(User user);
        Task<List<User>> GetBaseProfileUser(int? userId = null);
    }
}
