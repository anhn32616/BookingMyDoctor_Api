using booking_my_doctor.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace booking_my_doctor.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options) { }
        #region DB set
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Speciatly> Speciatlies { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        #endregion 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(e =>
            {
                e.Property(u => u.countViolation).HasDefaultValueSql(0.ToString());
                e.HasIndex(u => u.email).IsUnique();
            });
            modelBuilder.Entity<Role>(e => {
                e.HasMany(r => r.Users)
                .WithOne(u => u.role)
                .HasForeignKey(u => u.roleId);
            });
            modelBuilder.Entity<Doctor>(e =>
            {
                e.HasOne(d => d.clinic)
                .WithOne(c => c.doctor)
                .HasForeignKey<Doctor>(c => c.clinicId);
                e.HasOne(d => d.hospital)
                .WithMany(h => h.doctors)
                .HasForeignKey(e=> e.hospitalId);
                e.HasOne(e => e.speciatly)
                .WithMany(s => s.doctors)
                .HasForeignKey(d => d.specialtyId);
                e.HasOne(e => e.user)
                .WithOne()
                .HasForeignKey<Doctor>(e => e.userId);
            });
        }
    }
}
