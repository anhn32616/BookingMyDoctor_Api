using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Schedule
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public double Cost { get; set; }
        public virtual Doctor Doctor { get; set; }
        public virtual List<Appointment> Appointments { get; set; }
    }
}
