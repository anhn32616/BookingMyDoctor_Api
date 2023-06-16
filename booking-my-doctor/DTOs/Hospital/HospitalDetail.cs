using booking_my_doctor.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs.Hospital
{
    public class HospitalDetail
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(512)]
        public string name { get; set; }
        [Required, MaxLength(512)]
        public string address { get; set; }
        [Required, MaxLength(100)]
        public string city { get; set; }
        public string district { get; set; }
        public string ward { get; set; }
        [Required, MaxLength(1024)]
        public string imageUrl { get; set; }
        public virtual List<DoctorDto> doctors { get; set; } = new List<DoctorDto>();
    }
}
