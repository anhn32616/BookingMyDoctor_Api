using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Rate
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        public int Point { get; set; }
        [MaxLength(512)]
        public string? Comment { get; set; }
        public virtual Appointment Appointment { get; set; }
    }
}
