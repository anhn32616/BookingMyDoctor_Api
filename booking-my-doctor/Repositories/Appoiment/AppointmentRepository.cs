using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories.Appoiment
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly MyDbContext _context;

        public AppointmentRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            return true;
        }

        public async Task<bool> DeleteAppointment(Appointment appointment)
        {
            _context.Appointments.Remove(appointment);
            return true;
        }

        public async Task<Appointment> GetAppointmentById(int id)
        {
            return await _context.Appointments.Include(a => a.Schedule).Include(a => a.Patient).Include(a => a.Schedule.Doctor.user).FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<PaginationDTO<Appointment>> GetAppointments(int? page = null, int? pageSize = null, int? scheduleId = null, DateTime? date = null, string? status = null, int? patientId = null, int? doctorId = null, string? sortBy = "Date", bool? hiddenCancel = false)
        {
            var query = _context.Appointments.Include(a => a.Schedule).Include(a => a.Schedule.Doctor.user).Include(a => a.Patient).AsQueryable();
            if (scheduleId != null)
            {
                query = query.Where(u => u.ScheduleId == scheduleId);
            }
            if (date != null )
            {
                query = query.Where(u => u.date.Date == date.Value.Date);
            }
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(u => u.Status.Equals(status));
            }

            if (patientId != null)
            {
                query = query.Where(u => u.PatientId == patientId.Value);
            }

            if (doctorId != null)
            {
                query = query.Where(u => u.Schedule.DoctorId == doctorId.Value);
            }
            if (hiddenCancel != null && hiddenCancel == true)
            {
                query = query.Where(u => u.Status != "Cancel");
            }

            switch (sortBy)
            {
                case "Id":
                    query = query.OrderBy(u => u.Id);
                    break;
                case "Rating":
                    query = query.OrderByDescending(u => u.Rating);
                    break;
                case "Date":
                    query = query.OrderByDescending(u => u.date).ThenByDescending(u => u.Id);
                    break;
                default:
                    query = query.OrderByDescending(u => u.date);
                    break;
            }
            var pagination = new PaginationDTO<Appointment>();
            var appointments = new List<Appointment>();

            appointments = await query.ToListAsync();
            pagination.TotalCount = appointments.Count;
            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            } else
            {
                appointments = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = appointments;
            return pagination;
        }

        public async Task<bool> IsSaveChange()
        {
            return await _context.SaveChangesAsync() > 0;
        }


        public async Task<bool> UpdateAppointment(Appointment appointment)
        {
            _context.Entry(appointment).State= EntityState.Modified;
            return true;
        }

        public async Task<bool> UpdateAppointmentCancel()
        {
            // Hủy các appointment chưa được chấp nhận trước 1 giờ
            var appointmentCancel = await _context.Appointments.Where(a => a.date < DateTime.Now.AddHours(-1) && a.Status == "Pending").ToListAsync();
            foreach (var item in appointmentCancel)
            {
                item.Status = "Cancel";
            }
            return true;
        }

        public async Task<bool> UpdateAppointmentDone()
        {
            var appointmentCancel = await _context.Appointments.Where(a => a.date <= DateTime.Now && a.Status == "Confirm").ToListAsync();
            foreach (var item in appointmentCancel)
            {
                item.Status = "Done";
            }
            return true;
        }
    }
}
