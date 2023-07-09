using booking_my_doctor.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs.Notification
{
    public class NotificationDto
    {
        [Required]
        public int UserId { get; set; }
        [Required, MaxLength(2048)]
        public string Message { get; set; }
        [Required]
        public bool Read { get; set; }
        [Required]
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
