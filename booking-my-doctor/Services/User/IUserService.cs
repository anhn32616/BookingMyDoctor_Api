using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using MyWebApiApp.Models;

namespace booking_my_doctor.Services
{
    public interface IUserService
    {
        Task<ApiResponse> CreateUser(UserCreateDto user);
        Task<ApiResponse> GetAllUsers(int? page, int? pageSize, string? name, string sortColumn, string roleName);
        Task<ApiResponse> GetUserById(int? id);
        Task<ApiResponse> UpdateUser(int userId, UserUpdateDTO userUpdateDTO);
        Task<ApiResponse> DeleteUser(int userId);
        
    }
}
