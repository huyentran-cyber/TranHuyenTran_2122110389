using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Helpers;

namespace TranHuyenTran_2122110389.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 🔥 Seed Position trước
            modelBuilder.Entity<Position>().HasData(
                new Position
                {
                    Id = 1,
                    Name = "Admin",
                    HourlyRate = 0,
                    MinStaff = 1,
                    MaxShiftPerDay = 1
                },
                new Position
                {
                    Id = 2,
                    Name = "Staff",
                    HourlyRate = 100,
                    MinStaff = 1,
                    MaxShiftPerDay = 2
                }
            );

            // 🔥 Seed Admin
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Admin",
                    Email = "admin@gmail.com",
                    Phone = "0000000000",
                    Password = PasswordHelper.HashPassword("123456"),
                    Role = Role.Admin,
                    PositionId = 1
                }
            );
        }
    }
}