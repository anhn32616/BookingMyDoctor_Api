using booking_my_doctor.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.DTOs
{
    public class PaymentInfo
    {
        public double MonthlyFee { get; set; }
        public double AppointmentFee { get; set; }
        public double TotalFee => MonthlyFee + AppointmentFee;
    }
}
