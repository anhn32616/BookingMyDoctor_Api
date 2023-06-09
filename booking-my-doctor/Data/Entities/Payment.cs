﻿using System.ComponentModel.DataAnnotations;

namespace booking_my_doctor.Data.Entities
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public DateTime DatePayment { get; set; }
        [Required]
        public double MonthlyFee { get; set; }
        [Required]
        public double AppointmentFee { get; set; }
        public double TotalFee => MonthlyFee + AppointmentFee;

        [Required]
        public bool Status { get; set; } = false;
        [Required]
        public string? TransId { get; set; }
        public virtual Doctor Doctor { get; set; }

    }
}
