using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int userId { get; set; }
        [Range(1.0,5.0)]
        public double? rate { get; set; }
        public int? numberOfReviews { get; set; }
        [MaxLength(4096)]
        public string description { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? monthPaid { get; set; }
        [Required]
        public int hospitalId { get; set; }
        [Required]
        public int clinicId { get; set; }
        [Required]
        public int specialtyId { get; set; }
        public virtual Speciatly speciatly { get; set; }
        public virtual Hospital hospital { get; set; }
        public virtual Clinic clinic { get; set; }
        public virtual User user { get; set; }
        public virtual List<Schedule> Schedules { get; set; }
        public virtual List<Payment> Payments { get; set; }
        public virtual List<Timetable> Timetables { get; set; }
    }
}
