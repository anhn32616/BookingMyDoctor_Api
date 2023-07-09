using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int ScheduleId { get; set; }
        [Required]
        public DateTime date { get; set; }
        [Required]
        public string Symptoms { get; set; }
        [Required]
        public string Status { get; set; }
        public int? RateId { get; set; }
        public bool? Paid { get; set; }
        public virtual User Patient { get; set; }
        public virtual Schedule Schedule { get; set; }
        public virtual Rate? Rate { get; set; }
    }
}
