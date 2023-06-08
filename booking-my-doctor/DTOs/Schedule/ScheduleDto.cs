using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class ScheduleDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int DoctorId { get; set; }
        public bool? Status { get; set; } = false;

        [Required]
        public double Cost { get; set; }
    }
}
