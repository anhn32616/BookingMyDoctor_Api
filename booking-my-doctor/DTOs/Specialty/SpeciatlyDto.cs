using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class SpeciatlyDto
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(512)]
        public string name { get; set; }
        [MaxLength(1024)]
        public string imageUrl { get; set; } = "https://res.cloudinary.com/drotiisfy/image/upload/v1666932578/profiles/rddsnufikcvuvdp8e41l.jpg";
    }
}
