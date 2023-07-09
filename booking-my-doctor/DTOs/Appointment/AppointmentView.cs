using booking_my_doctor.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs.Appointment
{
    public class AppointmentView
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime date { get; set; }
        public string Symptoms { get; set; }
        public string Status { get; set; }
        public int? Rating { get; set; }
        public bool? Paid { get; set; }
        public UserDTO Patient { get; set; }
        public virtual ScheduleView Schedule { get; set; }
    }
}
