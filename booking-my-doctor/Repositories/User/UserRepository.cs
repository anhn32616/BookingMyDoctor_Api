using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;


namespace booking_my_doctor.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MyDbContext _context;

        public UserRepository(MyDbContext context)
        {
            _context = context;
        }
        public async Task<bool> CreateUser(User user)
        {
            await _context.Users.AddAsync(user);
            return await IsSaveChanges();
        }

        public async Task<bool> DeleteUser(User user)
        {
            _context.Users.Remove(user);
            return await IsSaveChanges();

        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.Include(u => u.role).FirstOrDefaultAsync(user => user.Id == id);
        }

        public async Task<PaginationDTO<User>> GetUsers(int? page, int? pageSize, string? name, string? sortColumn, string? roleName)
        {
            var query = _context.Users.Include(u => u.role).AsQueryable();

            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(u => u.fullName.Contains(name));
            }

            if (!string.IsNullOrEmpty(roleName))
            {
                query = query.Where(u => u.role.Name.Equals(roleName));
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
            var pagination = new PaginationDTO<User>();
            var users = new List<User>();

            users = await query.ToListAsync();
            pagination.TotalCount = users.Count;
            pagination.PageSize = users.Count;
            pagination.Page = 0;

            if (page != null && pageSize != null)
            {
                users = await query.Skip(page.Value * pageSize.Value).Take(pageSize.Value).ToListAsync();
                pagination.PageSize = pageSize.Value;
                pagination.Page = page.Value;
            }

            pagination.ListItem = users;
            return pagination;
        }

        public async Task<bool> UpdateUser(User user)
        {
            _context.Users.Update(user);
            return await IsSaveChanges();
        }

        public async Task<bool> IsSaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        public async Task<bool> IsEmailAlreadyExists(string email)
        {
            var currentUser = await _context.Users.FirstOrDefaultAsync(u => u.email.Equals(email));
            if (currentUser == null) { return false; }
            return true;
        }
        public async Task<User> GetUserByEmail(string email)
        {
            return await _context.Users.Include(u => u.role).FirstOrDefaultAsync(u => u.email == email);
        }

        public async Task<bool> VerifiedEmail(User user, string token)
        {
            if (user.token != token) return false;
            user.isEmailVerified = true;
            _context.Entry(user).State = EntityState.Modified;
            return true;
        }

        public async Task<int> GetLastUserID()
        {
            var id = _context.Users.OrderBy(u => u.Id).LastOrDefaultAsync().Result.Id;
            return id;
        }

        public async Task<bool> OpenCloseUser(User user)
        {
            user.isDelete = !user.isDelete;
            _context.Entry(user).State = EntityState.Modified;
            return true;
        }
    }
}
