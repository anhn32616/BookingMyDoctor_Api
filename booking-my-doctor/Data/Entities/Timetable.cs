using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Timetable
    {
        [Key]
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        [Required]
        public double Cost { get; set; }
        public int DoctorId { get; set; }
    }
}
