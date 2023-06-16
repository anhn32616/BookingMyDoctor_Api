using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace booking_my_doctor.Repositories
{
    public class ClinicRepository : IClinicRepository
    {
        private readonly MyDbContext _context;

        public ClinicRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationDTO<Clinic>> GetClinics(int? page, int? pageSize, string? keyword, string? sortColumn)
        {
            var query = _context.Clinics.AsQueryable();

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
            var pagination = new PaginationDTO<Clinic>();
            var clinics = new List<Clinic>();

            clinics = await query.ToListAsync();
            pagination.TotalCount = clinics.Count;

            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            }
            else
            {
                clinics = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = clinics;
            return pagination;
        }
        public async Task<Clinic> GetClinicById(int id)
        {
            var res = await _context.Clinics.Include(c => c.doctor).FirstOrDefaultAsync(c => c.Id == id);
            var doctors = await _context.Doctors.Where(d => d.clinicId == id).Include(d => d.hospital).Include(d => d.clinic).Include(d => d.speciatly).Include(d => d.user).FirstOrDefaultAsync();
            res.doctor = doctors;
            return res;
        }
        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateClinic(Clinic clinic)
        {
            await _context.Clinics.AddAsync(clinic);
            return true;
        }

        public async Task<bool> UpdateClinic(Clinic clinic)
        {
            _context.Entry(clinic).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteClinic(Clinic clinic)
        {
            _context.Remove(clinic);
            return true;
        }
    }
}
