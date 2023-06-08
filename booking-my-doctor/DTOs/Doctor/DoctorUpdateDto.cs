using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class DoctorUpdateDto
    {
        public UserUpdateDTO user { get; set; }

        [MaxLength(2048)]
        public string description { get; set; }

        [Required]
        public int hospitalId { get; set; }
        [Required]
        public int clinicId { get; set; }
        [Required]
        public int specialtyId { get; set; }
    }
}
