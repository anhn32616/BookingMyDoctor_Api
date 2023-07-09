using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs.Rate
{
    public class RateDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int AppointmentId { get; set; }
        [Required]
        public int Point { get; set; }
        [MaxLength(512)]
        public string? Comment { get; set; }
    }
}
