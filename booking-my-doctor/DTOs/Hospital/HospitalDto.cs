using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class HospitalDto
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
        public string imageUrl { get; set; } = "https://s3-rd-prod.crainsdetroit.com/s3fs-public/styles/1200x630/public/CMU%20medical%20school_i.jpg";
    }
}
