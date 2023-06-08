using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [MaxLength(256)]
        public string fullName { get; set; }

        [Required]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        public string password { get; set; }
    }
}
