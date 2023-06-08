using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Speciatly
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(512)]
        public string name { get; set; }
        [MaxLength(1024)]
        public string imageUrl { get; set; }
        public virtual List<Doctor> doctors { get; set; }

    }
}
