using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class UserCreateDto
    {
        [Required]
        [MaxLength(100)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MaxLength(256)]
        public string fullName { get; set; }
        public string? image { get; set; }
        [Required]
        public string roleName { get; set; }
        [Required]
        [MaxLength(12)]
        [Phone]
        public string phoneNumber { get; set; }
        public DateTime? birthDay { get; set; }
        [Required, MaxLength(100)]
        public string city { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        [MaxLength(1024)]
        public string? address { get; set; }
        public bool? gender { get; set; }
        [Range(0, 100)]
        public int? countViolation { get; set; }

    }
}