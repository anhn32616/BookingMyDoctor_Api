using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class RateRepository : IRateRepository
    {
        private readonly MyDbContext _context;

        public RateRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateRate(Rate rate)
        {
            await _context.Rates.AddAsync(rate);
            return true;
        }

        public async Task<Rate> GetRateByAppointmentId(int appointmentId)
        {
            return await _context.Rates.FirstOrDefaultAsync(r => r.Appointment.Id == appointmentId);
        }

        public async Task<List<Rate>> GetRates(int? doctorId = null)
        {
            var query = _context.Rates.Include(r => r.Appointment).Include(r => r.Appointment.Patient).AsQueryable();
            if(doctorId != null)
            {
                query = query.Where(r => r.Appointment.Schedule.DoctorId == doctorId);
            }
            return await query.ToListAsync();
        }

        public async Task<bool> IsSaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateRate(Rate rate)
        {
            _context.Entry(rate).State= EntityState.Modified;
            return true;
        }
    }
}
