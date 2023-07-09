using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class ScheduleView
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int DoctorId { get; set; }
        public int UserId { get; set; }
        public string Status { get; set; }

        [Required]
        public double Cost { get; set; }
        [Required]
        public string DoctorName { get; set; }
    }
}
