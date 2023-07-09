using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using booking_my_doctor.DTOs.User;
using Org.BouncyCastle.Asn1.Ocsp;


namespace booking_my_doctor.Services
{
    public interface IUserService
    {
        Task<ApiResponse> CreateUser(UserCreateDto user);
        Task<ApiResponse> GetAllUsers(int? page, int? pageSize, string? name, string sortColumn, string roleName);
        Task<ApiResponse> GetUserById(int? id);
        Task<ApiResponse> UpdateUser(int userId, UserUpdateDTO userUpdateDTO);
        Task<ApiResponse> DeleteUser(int userId);
        Task<ApiResponse> OpenCloseUser(int userId);
        Task<ApiResponse> GetBaseProfileUser(int? userId = null);
        Task<ApiResponse> GetAdminId();
        Task<ApiResponse> ChangePassUser(ChangepassDTO changepassDTO, int userId);


    }
}
