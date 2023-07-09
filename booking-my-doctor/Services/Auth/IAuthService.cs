using booking_my_doctor.DTOs;
using Org.BouncyCastle.Asn1.Ocsp;


namespace booking_my_doctor.Services
{
    public interface IAuthService
    {
        Task<ApiResponse> Login(UserLoginDto userLoginDto);
        Task<string> Register(RegisterUserDto registerUserDto);
        Task<ApiResponse> VerifiedEmail(string token);
        Task<ApiResponse> SendMailVerified(string email);
        Task<ApiResponse> ForgotPassword(string email);

    }
}
