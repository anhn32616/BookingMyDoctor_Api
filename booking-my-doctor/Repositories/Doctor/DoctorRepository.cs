using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly MyDbContext _context;

        public DoctorRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateDoctor(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            return true;
        }

        public async Task<bool> DeleteDoctor(Doctor doctor)
        {
            _context.Doctors.Remove(doctor);
            return true;
        }

        public async Task<Doctor> GetDoctorById(int id)
        {
            return await _context.Doctors.Where(d => d.Id == id).Include(d => d.hospital).Include(d => d.clinic).Include(d => d.speciatly).Include(d => d.user).Include(d => d.user.role).FirstOrDefaultAsync();
        }

        public async Task<int> GetDoctorIdByUserId(int userId)
        {
            return _context.Doctors.Where(d => d.userId == userId).FirstOrDefaultAsync().Result.Id;
        }

        public async Task<PaginationDTO<Doctor>> GetDoctors(int? page = 0, int? pageSize = int.MaxValue, string? keyword = null, string? sortColumn = "Id")
        {
            var query = _context.Doctors.Include(d => d.hospital).Include(d => d.clinic).Include(d => d.speciatly).Include(d => d.user).Include(d => d.user.role).AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.user.fullName.Contains(keyword));
            }

            switch (sortColumn)
            {
                case "Id":
                    query = query.OrderBy(u => u.Id);
                    break;
                default:
                    query = query.OrderBy(u => u.Id);
                    break;
            }
            var pagination = new PaginationDTO<Doctor>();
            var doctors = new List<Doctor>();

            doctors = await query.ToListAsync();
            pagination.TotalCount = doctors.Count;

            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            }
            else
            {
                doctors = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = doctors;
            return pagination;
        }

        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateDoctor(Doctor doctor)
        {
            _context.Entry(doctor).State = EntityState.Modified;
            return true;
        }
    }
}
