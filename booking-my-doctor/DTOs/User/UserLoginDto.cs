using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
    }
}
