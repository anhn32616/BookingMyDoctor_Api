using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly MyDbContext _context;


        public PaymentRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreatePaymentAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            return true;
        }

        public void DeletePayment(Payment payment)
        {
            _context.Payments.Remove(payment);
        }

        public async Task<PaginationDTO<Payment>> GetPaymentListAsync(int? page = null, int? pageSize = null, int? doctorId = null, bool? status = null, DateTime? date = null)
        {
            var query = _context.Payments.Include(p => p.Doctor).Include(p => p.Doctor.user).AsQueryable();
            if (doctorId != null)
            {
                query = query.Where(u => u.DoctorId == doctorId);
            }
            if (status != null)
            {
                query = query.Where(u => u.Status == status);
            }
            if (date != null)
            {
                query = query.Where(u => u.DatePayment.Date == date.Value.Date);
            }
            query = query.OrderByDescending(p => p.DatePayment);

            var pagination = new PaginationDTO<Payment>();
            var payments = new List<Payment>();

            payments = await query.ToListAsync();
            pagination.TotalCount = payments.Count;
            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            }
            else
            {
                payments = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = payments;
            return pagination;
        }


        public async Task<bool> IsSaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<Payment> GetPaymentById(int id)
        {
            return await _context.Payments.Include(p => p.Doctor).Include(p => p.Doctor.user).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<bool> IsPaymentSuccess(int paymentId)
        {
            return _context.Payments.FindAsync(paymentId).Result.Status;
        }

        public async Task<bool> CreatePayment(int doctorId)
        {
            var payment = new Payment
            {
                DoctorId = doctorId,
                DatePayment = DateTime.Now,
                MonthlyFee = 0,
                AppointmentFee = 0,
                Status = false,
                TransId = "",
            };
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if(doctor.monthPaid == null)
            {
                payment.MonthlyFee = 300000;
            } else 
            {
                if(doctor.monthPaid.Value.AddDays(30) < DateTime.Now) payment.MonthlyFee = 300000;
                var feeNotPaid = await _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done")
                    && a.date <= a.Schedule.Doctor.monthPaid.Value.AddDays(30) && a.Paid != true).SumAsync(a => a.Schedule.Cost);
                payment.AppointmentFee = feeNotPaid * 10/ 100;
            }
            _context.Payments.Add(payment);
            return true;
        }
        public async Task<PaymentInfo> GetPaymentInfo(int doctorId)
        {
            var payment = new PaymentInfo
            {
                MonthlyFee = 0,
                AppointmentFee = 0,
            };
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor.monthPaid == null)
            {
                payment.MonthlyFee = 300000;
            }
            else
            {
                if (doctor.monthPaid.Value.AddDays(30) < DateTime.Now) payment.MonthlyFee = 300000;
                var feeNotPaid = await _context.Appointments.Where(a => (a.Schedule.DoctorId == doctorId) && (a.Status == "Done")
                    && a.date <= a.Schedule.Doctor.monthPaid.Value.AddDays(30) && a.Paid != true).SumAsync(a => a.Schedule.Cost);
                payment.AppointmentFee = feeNotPaid * 10 / 100;
            }
            return payment;
        }

        public async Task<Payment> GetLastPayment()
        {
            return await _context.Payments.OrderByDescending(p => p.Id).FirstOrDefaultAsync();
        }

        public async Task<bool> HandlePaymentSuccess(int paymentId, string TranId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;
            payment.Status = true;
            payment.TransId = TranId;
            var listAppointment = await _context.Appointments.Where(a => (a.Schedule.DoctorId == payment.DoctorId) && (a.Status == "Done")
                    && a.date <= a.Schedule.Doctor.monthPaid.Value.AddDays(30) && a.Paid != true).ToListAsync();
            foreach (var item in listAppointment)
            {
                item.Paid = true;
            }
            if(payment.MonthlyFee != 0)
            {
                var doctor = await _context.Doctors.FindAsync(payment.DoctorId);
                if (doctor == null) return false;
                doctor.monthPaid = payment.DatePayment;
            };
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> HandlePaymentFailure(int paymentId)
        {
            var payment = await _context.Payments.FindAsync(paymentId);
            if (payment == null) return false;
            payment.Status = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
