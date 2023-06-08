using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly MyDbContext _context;

        public RoleRepository(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Role>> getAll()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<Role> getRoleByName(string name)
        {
            var result =  _context.Roles.FirstOrDefault(r => r.Name == name);
            return result;
        }
    }
}
