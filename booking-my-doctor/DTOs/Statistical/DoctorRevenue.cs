using booking_my_doctor.DTOs.Appointment;

namespace booking_my_doctor.DTOs
{
    public class DoctorRevenue
    {
        public int DoctorId { get; set; }
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int TotalAppointmentDone { get; set; }
        public double Revenue { get; set; }
        public double Profit { get; set; }
        public double Fee { get; set; }
        public double FeePaid { get; set; }
        public List<AppointmentView>? Appointments { get; set; }
    }
}
