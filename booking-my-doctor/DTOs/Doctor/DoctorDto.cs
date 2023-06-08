using booking_my_doctor.DTOs;
using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class DoctorDto
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int userId { get; set; }
        [Range(1.0,5.0)]
        public double? rate { get; set; }
        public int? numberOfReviews { get; set; }
        [MaxLength(2048)]
        public string description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? monthPaid { get; set; }
        [Required]
        public int hospitalId { get; set; }
        [Required]
        public int clinicId { get; set; }
        [Required]
        public int specialtyId { get; set; }
        public virtual SpeciatlyDto speciatly { get; set; }
        public virtual HospitalDto hospital { get; set; }
        public virtual ClinicDto clinic { get; set; }
        public virtual UserDTO user { get; set; }
    }
}
