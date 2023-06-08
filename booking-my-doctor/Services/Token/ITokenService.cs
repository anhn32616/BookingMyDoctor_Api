namespace booking_my_doctor.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(string email);
    }
}
