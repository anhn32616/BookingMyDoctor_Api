using MailKit.Security;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MimeKit;
using booking_my_doctor.Repositories;
using booking_my_doctor.DTOs;

namespace booking_my_doctor.Services 
{ 
    public class EmailService : IEmailService
    {

        /// <summary>
        /// Get Email config in appsettings.json 
        /// </summary>
        private readonly IConfiguration _config;
        public EmailService(IUserRepository userRepository, ITokenService tokenService, IConfiguration config)
        {
            _config = config;
        }
        /// <summary>
        /// Send new email to user 
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        public ApiResponse SendEmail(string to, string subject, string body)
        {
            try
            {
                //Create email 
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_config.GetSection("Email:From").Value));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Text) { Text = body };

                using var smtp = new SmtpClient();


                //Config smtp to send email 
                smtp.Connect(_config.GetSection("Email:Host").Value, int.Parse(_config.GetSection("Email:Port").Value), SecureSocketOptions.StartTls);
                smtp.Authenticate(_config.GetSection("Email:From").Value, _config.GetSection("Email:Password").Value);
                smtp.Send(email);
                smtp.Disconnect(true);
                return new ApiResponse()
                {
                    statusCode = 200,
                    message = "Thành công! Please check your email",
                };
            }
            catch (Exception e)
            {
                return new ApiResponse()
                {
                    statusCode = 400,
                    message = "Faile. " + e.Message,
                };
            }
        }
    }
}
