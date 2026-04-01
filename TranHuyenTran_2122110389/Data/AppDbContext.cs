using ConnectDB.Models;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Models;

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
    }
}