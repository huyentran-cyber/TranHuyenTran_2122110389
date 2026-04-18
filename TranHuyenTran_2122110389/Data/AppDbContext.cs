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

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Shift> Shifts { get; set; }
        public DbSet<WorkSchedule> WorkSchedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Salary> Salaries { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.WorkSchedule)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.ScheduleId)
                .OnDelete(DeleteBehavior.Restrict); // Chuyển từ Cascade sang Restrict

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Employee)
                .WithMany(e => e.Attendances)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            //cấu hình trạng thái cho nhân viên, mặc định là true (đang làm việc)
            modelBuilder.Entity<Employee>()
                .Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Cấu hình decimal cho bảng Position
            modelBuilder.Entity<Position>()
                .Property(p => p.HourlyRate)
                .HasColumnType("decimal(18,2)");

            // Cấu hình mối quan hệ giữa Shift và Position
            modelBuilder.Entity<Shift>()
                .HasOne(s => s.Position)
                .WithMany(p => p.Shifts) 
                .HasForeignKey(s => s.PositionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Cấu hình decimal cho bảng Salary
            modelBuilder.Entity<Salary>(entity => {
                entity.Property(e => e.TotalHours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.HourlyRateAtTime).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PenaltyViolation).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PenaltyAbsent).HasColumnType("decimal(18,2)");
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            });
            // 🔥 Seed Position trước
            modelBuilder.Entity<Position>().HasData(
                new Position
                {
                    Id = 1,
                    Name = "Quản lý",
                    HourlyRate = 0,
                    MinStaff = 1,
                    MaxShiftPerDay = 1
                }
                
            );

            // 🔥 Seed Admin
            modelBuilder.Entity<Employee>().HasData(
                new Employee
                {
                    Id = 1,
                    Name = "Trần Huyền Trân",
                    Email = "huyentran@gmail.com",
                    Phone = "0912345678",
                    Password = PasswordHelper.HashPassword("123456"),
                    Role = Role.Manager,
                    PositionId = 1
                }
            );
        }
    }
}