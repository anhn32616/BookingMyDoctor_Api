using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class ClinicDto
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
        public string imageUrl { get; set; } = "https://www.dcms.uscg.mil/Portals/10/DOL/BaseNCR/img/clinicPhoto.png";
    }
}
