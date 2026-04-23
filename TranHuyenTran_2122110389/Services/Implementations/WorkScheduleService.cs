using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Controllers;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Controllers;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class WorkScheduleService : IWorkScheduleService
    {
        private readonly AppDbContext _context;

        public WorkScheduleService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<WorkSchedule> RegisterAsync(int employeeId, int shiftId, DateTime date)
        {
            if (!WorkScheduleController.RegistrationOpen)
            {
                throw new Exception("Hệ thống hiện đang đóng cổng đăng ký ca làm. Vui lòng quay lại sau hoặc liên hệ Quản lý!");
            }
            // 1. Lấy thông tin nhân viên và vị trí (để biết là Phục vụ hay Pha chế)
            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) throw new Exception("Nhân viên không tồn tại.");

            var position = employee.Position;

            // ĐẾM XEM ĐÃ CÓ BAO NHIÊU NGƯỜI ĐĂNG KÝ CA NÀY, NGÀY NÀY RỒI
            int currentStaffCount = await _context.WorkSchedules
                .CountAsync(s => s.ShiftId == shiftId
                            && s.WorkDate.Date == date.Date
                            && s.Status != "Rejected");

            if (currentStaffCount >= position.MinStaff)
            {
                throw new Exception($"Lỗi: Ca làm này vào ngày {date:dd/MM} đã đủ nhân sự ({position.MinStaff} người). Vui lòng chọn ca khác!");
            }

            // 2. LOGIC TÍNH TOÁN VÀ CHẶN THEO TUẦN (Thứ 2 - Chủ nhật)
            // Tìm ngày Thứ 2 của tuần chứa ngày 'date' nhân viên đang chọn
            int diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = date.AddDays(-1 * diff).Date; // Ngày Thứ 2 đầu tuần
            DateTime endOfWeek = startOfWeek.AddDays(6).AddHours(23).AddMinutes(59); // Ngày Chủ nhật cuối tuần

            // Đếm tổng số ca nhân viên đã đăng ký trong tuần đó
            int weeklyCount = await _context.WorkSchedules
                .Where(s => s.EmployeeId == employeeId
                       && s.WorkDate >= startOfWeek
                       && s.WorkDate <= endOfWeek)
                .CountAsync();

            // CẢNH BÁO: Nếu đã đạt giới hạn 10 ca thì chặn ngay
            if (weeklyCount >= 10)
            {
                throw new Exception($"CẢNH BÁO: Bạn đã đăng ký đủ định mức 10 ca cho tuần từ {startOfWeek:dd/MM} đến {endOfWeek:dd/MM}. Không thể đăng ký thêm!");
            }

            // 3. LOGIC CHẶN THEO NGÀY (Trùng giờ và MaxShiftPerDay)
            var dateOnly = date.Date;
            var todaySchedules = await _context.WorkSchedules
                .Include(s => s.Shift)  
                .Where(s => s.EmployeeId == employeeId
                    && s.WorkDate.Year == dateOnly.Year
                    && s.WorkDate.Month == dateOnly.Month
                    && s.WorkDate.Day == dateOnly.Day)
                .ToListAsync();

            var currentShift = await _context.Shifts.FindAsync(shiftId);
            foreach (var item in todaySchedules)
            {
                if (currentShift.StartTime < item.Shift.EndTime && currentShift.EndTime > item.Shift.StartTime)
                {
                    throw new Exception("Lỗi: Ca đăng ký bị trùng thời gian với ca đã có trong ngày.");
                }
            }

            int maxAllowed = employee.Position.MaxShiftPerDay;

            if (todaySchedules.Count >= maxAllowed)
            {
                throw new Exception($"Lỗi: Theo quy định, vị trí {employee.Position.Name} chỉ được đăng ký tối đa {maxAllowed} ca/ngày.");
            }

            // 4. Lưu đăng ký
            var newSchedule = new WorkSchedule
            {
                EmployeeId = employeeId,
                ShiftId = shiftId,
                WorkDate = dateOnly,
                Status = "Pending"
            };

            _context.WorkSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();
            return newSchedule;
        }

        public async Task<IEnumerable<WorkScheduleDTO>> GetMySchedulesAsync(int employeeId, int month, int year)
        {
            return await _context.WorkSchedules
                .Include(s => s.Shift)
                .Include(s => s.Attendances) // Đảm bảo nạp dữ liệu điểm danh
                .Where(s => s.EmployeeId == employeeId &&
                            s.WorkDate.Month == month &&
                            s.WorkDate.Year == year)
                .OrderByDescending(s => s.WorkDate)
                .Select(s => new WorkScheduleDTO
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    ShiftId = s.ShiftId,
                    ShiftName = s.Shift != null ? s.Shift.Name : "N/A",
                    StartTime = s.Shift != null ? s.Shift.StartTime : TimeSpan.Zero,
                    EndTime = s.Shift != null ? s.Shift.EndTime : TimeSpan.Zero,
                    WorkDate = s.WorkDate,
                    Status = s.Status,
                    // Lấy dữ liệu chấm công an toàn bằng cách Select từng trường trước khi FirstOrDefault
                    CheckInTime = s.Attendances.Select(a => (DateTime?)a.CheckIn).FirstOrDefault(),
                    CheckOutTime = s.Attendances.Select(a => a.CheckOut).FirstOrDefault(),
                    AttendanceStatus = s.Attendances.Select(a => a.Status).FirstOrDefault()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<Shift>> GetAvailableShiftsForEmployeeAsync(int employeeId)
        {
            // 1. Tìm nhân viên để biết họ thuộc Vị trí (Position) nào
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) return new List<Shift>();

            // 2. Lấy danh sách ca làm dựa trên PositionId của nhân viên
            // Giả sử bảng Shift của bạn có cột PositionId để phân loại
            return await _context.Shifts
                .Where(s => s.PositionId == employee.PositionId)
                .ToListAsync();
        }
        // Trong WorkScheduleService.cs

        public async Task<IEnumerable<WorkScheduleDTO>> GetPendingSchedulesAsync(DateTime? date = null, int? positionId = null)
        {
            var query = _context.WorkSchedules
                .Include(s => s.Employee).ThenInclude(e => e.Position)
                .Include(s => s.Shift)
                .Where(s => s.Status == "Pending")
                .AsQueryable();

            // Nếu phía Frontend có truyền ngày (date), thì lọc đúng ngày đó
            if (date.HasValue)
            {
                query = query.Where(s => s.WorkDate.Date == date.Value.Date);
            }
            // Lọc theo Vị trí 
            if (positionId.HasValue && positionId > 0)
                query = query.Where(s => s.Employee.PositionId == positionId.Value);

            return await query
                .Select(s => new WorkScheduleDTO
                {
                    Id = s.Id,
                    EmployeeId = s.EmployeeId,
                    ShiftId = s.ShiftId,
                    EmployeeName = s.Employee.Name,
                    PositionName = s.Employee.Position != null ? s.Employee.Position.Name : "N/A",
                    ShiftName = s.Shift.Name,
                    StartTime = s.Shift.StartTime,
                    EndTime = s.Shift.EndTime,
                    WorkDate = s.WorkDate,
                    Status = s.Status
                })
                .ToListAsync();
        }

        public async Task<bool> ConfirmScheduleAsync(int scheduleId)
        {
            var schedule = await _context.WorkSchedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            // Cập nhật trạng thái thành chuỗi "Confirmed" để khớp với Model
            schedule.Status = "Confirmed";

            _context.WorkSchedules.Update(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RejectScheduleAsync(int scheduleId)
        {
            var schedule = await _context.WorkSchedules.FindAsync(scheduleId);
            if (schedule == null) return false;

            // Cập nhật trạng thái thành Rejected
            schedule.Status = "Rejected";

            _context.WorkSchedules.Update(schedule);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
