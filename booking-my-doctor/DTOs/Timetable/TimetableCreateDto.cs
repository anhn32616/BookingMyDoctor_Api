using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs.Timetable
{
    public class TimetableCreateDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public double Cost { get; set; }
        [Required]
        public int Count { get; set; } = 1;
    }
}
