using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<AttendanceDTO> GetAll()
        {
            return _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.CheckIn)
                .Select(a => new AttendanceDTO // Bước Mapping từ Model sang DTO
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    CheckIn = a.CheckIn,
                    CheckOut = a.CheckOut,
                    Status = a.Status
                })
                .ToList();
        }

        public async Task<AttendanceDTO> CheckInAsync(int employeeId)
        {
            var now = DateTime.Now;
            var today = now.Date;

            var schedule = await _context.WorkSchedules
                .Include(s => s.Shift)
                .FirstOrDefaultAsync(s => s.EmployeeId == employeeId
                                     && s.WorkDate.Date == today
                                     && s.Status == "Confirmed");

            if (schedule == null)
                throw new Exception("Bạn không có lịch làm việc được xác nhận trong hôm nay.");

            var exists = await _context.Attendances.AnyAsync(a => a.ScheduleId == schedule.Id);
            if (exists) throw new Exception("Bạn đã điểm danh vào ca này rồi.");

            bool isLate = now.TimeOfDay > schedule.Shift.StartTime.Add(TimeSpan.FromMinutes(5));

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                ScheduleId = schedule.Id,
                CheckIn = now,
                IsLate = isLate,
                IsEarly = false,
                Status = isLate ? "Late" : "Present"
            };

            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();

            // Trả về DTO thay vì Model
            return new AttendanceDTO
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                CheckIn = attendance.CheckIn,
                Status = attendance.Status
            };
        }

        public async Task<AttendanceDTO> CheckOutAsync(int employeeId)
        {
            var attendance = await _context.Attendances
                .Include(a => a.WorkSchedule).ThenInclude(s => s.Shift)
                .OrderByDescending(a => a.CheckIn)
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.CheckOut == null);

            if (attendance == null)
                throw new Exception("Không tìm thấy dữ liệu Check-in để thực hiện Check-out.");

            var now = DateTime.Now;

            if (now.TimeOfDay < attendance.WorkSchedule.Shift.EndTime)
            {
                attendance.IsEarly = true;
                attendance.Status = "Early Leave"; // Cập nhật lại status nếu về sớm
            }

            attendance.CheckOut = now;
            await _context.SaveChangesAsync();

            return new AttendanceDTO
            {
                Id = attendance.Id,
                EmployeeId = attendance.EmployeeId,
                CheckIn = attendance.CheckIn,
                CheckOut = attendance.CheckOut,
                Status = attendance.Status
            };
        }
    }
}