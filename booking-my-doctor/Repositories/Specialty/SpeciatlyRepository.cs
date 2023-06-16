using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace booking_my_doctor.Repositories
{
    public class SpeciatlyRepository : ISpeciatlyRepository
    {
        private readonly MyDbContext _context;

        public SpeciatlyRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PaginationDTO<Speciatly>> GetSpeciatlies(int? page, int? pageSize, string? keyword, string? sortColumn)
        {
            var query = _context.Speciatlies.AsQueryable();

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
            var pagination = new PaginationDTO<Speciatly>();
            var speciatlys = new List<Speciatly>();

            speciatlys = await query.ToListAsync();
            pagination.TotalCount = speciatlys.Count;

            if (page == null || pageSize == null)
            {
                pagination.Page = 0;
                pagination.PageSize = (pagination.TotalCount != 0) ? pagination.TotalCount : 10;
            }
            else
            {
                speciatlys = await query.Skip(page!.Value * pageSize!.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }
            pagination.ListItem = speciatlys;
            return pagination;
        }
        public async Task<Speciatly> GetSpeciatlyById(int id)
        {
            var res = await _context.Speciatlies.FirstOrDefaultAsync(c => c.Id == id);
            var doctors = await _context.Doctors.Where(d => d.specialtyId == id).Include(d => d.hospital).Include(d => d.clinic).Include(d => d.speciatly).Include(d => d.user).ToListAsync();
            res.doctors = doctors;
            return res;
        }
        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateSpeciatly(Speciatly speciatly)
        {
            await _context.Speciatlies.AddAsync(speciatly);
            return true;
        }

        public async Task<bool> UpdateSpeciatly(Speciatly speciatly)
        {
            _context.Entry(speciatly).State = EntityState.Modified;
            return true;
        }

        public async Task<bool> DeleteSpeciatly(Speciatly speciatly)
        {
            _context.Remove(speciatly);
            return true;
        }
    }
}
