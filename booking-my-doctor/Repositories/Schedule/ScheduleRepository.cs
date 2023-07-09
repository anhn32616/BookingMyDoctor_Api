using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;


namespace booking_my_doctor.Repositories
{
    public class ScheduleRepository : IScheduleRepository
    {
        private readonly MyDbContext _context;

        public ScheduleRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationDTO<ScheduleView>> GetSchedules(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null, string? status = null, DateTime? date = null, string? sortColumn = "StartTime")
        {
            var query = _context.Schedules.AsQueryable();

            if (doctorId != null)
            {
                query = query.Where(s => s.DoctorId == doctorId);
            }
            if (date != null)
            {
                query = query.Where(s => s.StartTime.Date == date.Value.Date);
            }
            if (status != null)
            {
                query = query.Where(s => s.Status.Equals(status));
            }

            switch (sortColumn)
            {
                case "Id":
                    query = query.OrderBy(u => u.Id);
                    break;
                case "StartTime":
                    query = query.OrderByDescending(u => u.StartTime);
                    break;
                default:
                    query = query.OrderBy(u => u.Id);
                    break;
            }
            var pagination = new PaginationDTO<ScheduleView>();
            var schedules = new List<Schedule>();

            schedules = await query.ToListAsync();
            pagination.TotalCount = schedules.Count;

            var schedulesView = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).Select(s => new ScheduleView
            {
                Id = s.Id,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Cost = s.Cost,
                Status = s.Status,
                DoctorId = s.DoctorId,
                DoctorName = s.Doctor.user.fullName
            }).ToListAsync();
            pagination.PageSize = pageSize.Value;
            pagination.Page = page.Value;
            pagination.ListItem = schedulesView;
            return pagination;
        }
        public async Task<Schedule> GetScheduleById(int id)
        {
            return await _context.Schedules.Include(s => s.Doctor.user).FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateSchedule(Schedule schedule)
        {
            await _context.Schedules.AddAsync(schedule);
            return true;
        }

        public async Task<bool> UpdateSchedule(Schedule schedule)
        {
            _context.Entry(schedule).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteSchedule(Schedule schedule)
        {
            _context.Remove(schedule);
            return true;
        }

        public async Task<bool> UpdateStatusSchedule(Schedule schedule)
        {
            var count = await _context.Appointments.Where(a => a.ScheduleId == schedule.Id && a.Status == "Pending").CountAsync();
            if (count == 0) schedule.Status = "Available";
            else schedule.Status = "Pending";
            _context.Entry(schedule).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> UpdateScheduleExpired()
        {
            var schedules = await _context.Schedules.Where(s => (s.Status.Equals("Available") || (s.Status.Equals("Pending") && s.Appointments.All(a => a.Status.Equals("Cancel"))))
            && s.StartTime < DateTime.Now).ToListAsync();
            foreach (var item in schedules)
            {
                item.Status = "Expired";
                _context.Entry(item).State = EntityState.Modified;
            }
            return true;
        }

        public async Task<bool> AutoAddSchedule()
        {
            var doctors = await _context.Doctors.Include(d => d.Timetables).Include(d => d.Schedules).ToListAsync();
            foreach (var doctor in doctors)
            {
                var today = DateTime.Now;
                for (int i = 1; i <= 7; i++)
                {
                    var day = today.AddDays(i);
                    if(!doctor.Schedules.Any(s => s.StartTime.Date == day.Date))
                    {
                        foreach (var timetalbe in doctor.Timetables)
                        {
                            var diffday = day.Date - timetalbe.StartTime.Date;
                            var schedule = new Schedule
                            {
                                DoctorId = doctor.Id,
                                Cost = timetalbe.Cost,
                                StartTime = timetalbe.StartTime.Add(diffday),
                                EndTime = timetalbe.EndTime.Add(diffday),
                                Status = "Available"
                            };
                            doctor.Schedules.Add(schedule);
                        }
                    }
                }
                _context.Entry(doctor).State = EntityState.Modified;
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
