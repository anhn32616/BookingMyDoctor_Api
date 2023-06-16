using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class ScheduleCreateDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        public int? DoctorId { get; set; }
        [Required]
        public int Count { get; set; } = 1;
        [Required]
        public double Cost { get; set; }
    }
}
