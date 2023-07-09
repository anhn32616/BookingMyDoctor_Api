using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace booking_my_doctor.Data.Entities
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string email { get; set; }

        [Required]
        [MaxLength(256)]
        public string fullName { get; set; }
        public string? image { get; set; }
        [MaxLength(12)]
        [Phone]
        public string? phoneNumber { get; set; }
        public DateTime? birthDay { get; set; }
        [MaxLength(100)]
        public string? city { get; set; }
        public string? district { get; set; }
        public string? ward { get; set; }
        [MaxLength(1024)]
        public string? address { get; set; }
        public bool? isDelete { get; set; }
        [Required]
        [Range(0, 100)]
        public int roleId { get; set; }
        public virtual Role role { get; set; }
        [MaxLength(512)]
        public string? token { get; set; }

        [Range(0, 100)]

        public int? countViolation { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public bool? gender { get; set; }
        public bool isEmailVerified { get; set; }
        public virtual List<Appointment>? Appointments { get; set; }
        public virtual List<Notification> Notifications { get; set; }
    }
}
