using booking_my_doctor.DTOs;
using MyWebApiApp.Models;

namespace booking_my_doctor.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(UserLoginDto userLoginDto);
        Task<string> Register(RegisterUserDto registerUserDto);
        Task<ApiResponse> VerifiedEmail(string email, string token);
        Task<ApiResponse> SendMailVerified(string email);
    }
}
