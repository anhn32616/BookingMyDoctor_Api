using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class TimetableRepository : ITimetableRepository
    {
        private readonly MyDbContext _context;

        public TimetableRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationDTO<Timetable>> GetTimetables(int? page = 0, int? pageSize = int.MaxValue, int? doctorId = null)
        {
            var query = _context.Timetables.AsQueryable();

            if (doctorId != null)
            {
                query = query.Where(s => s.DoctorId == doctorId);
            }
            query = query.OrderBy(u => u.StartTime);

            var pagination = new PaginationDTO<Timetable>();
            var timetables = new List<Timetable>();

            timetables = await query.ToListAsync();
            pagination.TotalCount = timetables.Count;

            var timetablesView = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
            pagination.PageSize = pageSize.Value;
            pagination.Page = page.Value;
            pagination.ListItem = timetablesView;
            return pagination;
        }
        public async Task<Timetable> GetTimetableById(int id)
        {
            return await _context.Timetables.FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateTimetable(Timetable timetable)
        {
            await _context.Timetables.AddAsync(timetable);
            return true;
        }

        public async Task<bool> UpdateTimetable(Timetable timetable)
        {
            _context.Entry(timetable).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteTimetable(Timetable timetable)
        {
            _context.Remove(timetable);
            return true;
        }
    }
}
