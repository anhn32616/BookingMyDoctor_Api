using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs.Appointment
{
    public class AppointmentCreate
    {
        public int? PatientId { get; set; }
        [Required]
        public int ScheduleId { get; set; }
        //public string? Status { get; set; }
        [Required]
        public string Symptoms { get; set; }
    }
}
