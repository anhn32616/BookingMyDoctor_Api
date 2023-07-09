using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int UserId { get; set; }
        [Required, MaxLength(2048)]
        public string Message { get; set; }
        [Required]
        public bool Status { get; set; }
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public virtual User User { get; set; }
    }
}
