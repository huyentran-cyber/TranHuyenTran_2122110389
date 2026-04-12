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

        private string FormatDuration(int totalMinutes)
        {
            if (totalMinutes < 60)
            {
                return $"{totalMinutes}m"; // Kết quả ví dụ: 45p
            }

            int hours = totalMinutes / 60;
            int mins = totalMinutes % 60;

            // Nếu có phút lẻ thì hiện cả tiếng cả phút, nếu không thì chỉ hiện tiếng
            if (mins > 0)
            {
                return $"{hours}h {mins}m"; 
            }
            return $"{hours}h"; 
        }

        public AttendanceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceDTO>> GetAllByDateAsync(DateTime date)
        {
            // 1. Lấy danh sách từ DB kèm theo các bảng liên quan
            var list = await _context.Attendances
                .Include(a => a.Employee)
                    .ThenInclude(e => e.Position)
                .Include(a => a.WorkSchedule)
                    .ThenInclude(ws => ws.Shift)
                .Where(a => a.CheckIn.Date == date.Date)
                .OrderByDescending(a => a.CheckIn)
                .ToListAsync();

            // 2. Mapping sang DTO - Bước này cực kỳ quan trọng để hiện tên trên Web
            return list.Select(a => new AttendanceDTO
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,

                // Gán tên nhân viên (Fix lỗi trống cột Nhân viên)
                EmployeeName = a.Employee?.Name ?? "N/A",

                // Gán tên ca làm (Fix lỗi hiện N/A ở Ca làm)
                ShiftName = a.WorkSchedule?.Shift?.Name ?? "N/A",

                // Gán tên vị trí
                PositionName = a.Employee?.Position?.Name ?? "Staff",

                // Lấy giờ quy định để hiện dòng nhỏ dưới tên ca
                StartTime = a.WorkSchedule?.Shift?.StartTime ?? TimeSpan.Zero,
                EndTime = a.WorkSchedule?.Shift?.EndTime ?? TimeSpan.Zero,

                CheckIn = a.CheckIn,
                CheckOut = a.CheckOut,
                Status = a.Status
            }).ToList();
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

            // --- LOGIC TÍNH PHÚT ĐI MUỘN ---
            var shiftStart = schedule.Shift.StartTime;
            var currentTime = now.TimeOfDay;
            bool isLate = currentTime > shiftStart.Add(TimeSpan.FromMinutes(5));

            string statusText = "Present";
            if (isLate)
            {
                // Tính số phút đi muộn
                int lateMinutes = (int)(currentTime - shiftStart).TotalMinutes;
                statusText = $"Late ({FormatDuration(lateMinutes)})";
            }

            var attendance = new Attendance
            {
                EmployeeId = employeeId,
                ScheduleId = schedule.Id,
                CheckIn = now,
                IsLate = isLate,
                IsEarly = false,
                Status = statusText
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
            // Tìm bản ghi check-in mới nhất hôm nay mà chưa có check-out
            var attendance = await _context.Attendances
                .Include(a => a.WorkSchedule).ThenInclude(s => s.Shift)
                .Where(a => a.EmployeeId == employeeId && a.CheckOut == null)
                .OrderByDescending(a => a.CheckIn)
                .FirstOrDefaultAsync();

            if (attendance == null)
                throw new Exception("Không tìm thấy dữ liệu Check-in để thực hiện Check-out.");

            var now = DateTime.Now;
            var shiftEnd = attendance.WorkSchedule.Shift.EndTime;
            var currentTime = now.TimeOfDay;

            // --- LOGIC TÍNH PHÚT VỀ SỚM ---
            if (currentTime < shiftEnd)
            {
                attendance.IsEarly = true;
                int earlyMinutes = (int)(shiftEnd - currentTime).TotalMinutes;

                // Nếu đã đi muộn thì nối thêm chuỗi, nếu không thì ghi mới
                string earlyText = $"Early ({FormatDuration(earlyMinutes)})";

                if (attendance.IsLate)
                    attendance.Status += $" & {earlyText}";
                else
                    attendance.Status = earlyText;
            }
            else if (string.IsNullOrEmpty(attendance.Status) || !attendance.IsLate)
            {
                attendance.Status = "Present";
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