using MyWebApiApp.Models;

namespace booking_my_doctor.Services
{
     
    public interface IEmailService
    {
        /// <summary>
        /// Send new email to user 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        ApiResponse SendEmail(string to, string subject, string body);
    }
}
