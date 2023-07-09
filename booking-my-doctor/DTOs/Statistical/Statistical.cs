
namespace booking_my_doctor.DTOs
{
    public class Statistical
    {
        public double CompanyRevenue { get; set; }
        public double TotalMonthlyFee { get; set; }
        public double DoctorRevenue { get; set; }
        public double DoctorProfit { get; set; }
        public int TotalAppointmentDone { get; set; }
        public int TotalDoctorActivity { get; set; }
        public double FeeAppointment { get; set; }
        public double FeeAppointmentPaid { get; set; }
        public PaginationDTO<DoctorRevenue> PaginationDoctorRevenues { get; set; }
    }
}
