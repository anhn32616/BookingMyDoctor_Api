using booking_my_doctor.Data;
using booking_my_doctor.Data.Entities;
using booking_my_doctor.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace DatingApp.API.Data.Seed
{
    public class Seed 
    {
        public static void SeedUsers(MyDbContext context)
        {     
            if (context.Roles.Any()) return;
            Role roleAdmin = new Role() { Name = "ROLE_ADMIN" };
            List<Role> roles = new List<Role>()
            {
                roleAdmin,
                new Role() {Name="ROLE_DOCTOR"},
                new Role() {Name="ROLE_PATIENT"},
            };
            context.Roles.AddRange(roles);
            var admin = new User();
            admin.email = "bookingmydoctor942@gmail.com";
            admin.fullName = "ADMIN";
            admin.image = "https://static.vecteezy.com/system/resources/thumbnails/019/194/935/small/global-admin-icon-color-outline-vector.jpg";
            admin.city = "Hà Tĩnh";
            admin.district = "";
            admin.ward = "";
            admin.address = "";
            admin.role = roleAdmin;
            admin.isEmailVerified = true;
            using var hmac = new HMACSHA512();
            var passwordBytes = Encoding.UTF8.GetBytes("Admin123@");
            admin.PasswordSalt = hmac.Key;
            admin.PasswordHash = hmac.ComputeHash(passwordBytes);
            context.Users.Add(admin);
            context.SaveChanges();
        }
    }
}