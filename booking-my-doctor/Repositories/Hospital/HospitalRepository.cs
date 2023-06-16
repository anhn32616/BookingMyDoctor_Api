using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace booking_my_doctor.Repositories
{
    public class HospitalRepository : IHospitalRepository
    {
        private readonly MyDbContext _context;

        public HospitalRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationDTO<Hospital>> GetHospitals(int? page, int? pageSize, string? keyword, string? sortColumn)
        {
            var query = _context.Hospitals.AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(u => u.name.Contains(keyword));
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
            var pagination = new PaginationDTO<Hospital>();
            var hospitals = new List<Hospital>();

            hospitals = await query.ToListAsync();
            pagination.TotalCount = hospitals.Count;

            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            }
            else
            {
                hospitals = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = hospitals;
            return pagination;
        }
        public async Task<Hospital> GetHospitalById(int id)
        {
            var res = await _context.Hospitals.FirstOrDefaultAsync(c => c.Id == id);
            var doctors = await _context.Doctors.Where(d => d.hospitalId == id).Include(d => d.hospital).Include(d => d.clinic).Include(d => d.speciatly).Include(d => d.user).ToListAsync();
            res.doctors = doctors;
            return res;
        }
        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateHospital(Hospital hospital)
        {
            await _context.Hospitals.AddAsync(hospital);
            return true;
        }

        public async Task<bool> UpdateHospital(Hospital hospital)
        {
            _context.Entry(hospital).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteHospital(Hospital hospital)
        {
            _context.Remove(hospital);
            return true;
        }
    }
}
