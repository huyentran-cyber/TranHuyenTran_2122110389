using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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
            // 1. Lấy thông tin nhân viên và vị trí (để biết là Phục vụ hay Pha chế)
            var employee = await _context.Employees
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == employeeId);

            if (employee == null) throw new Exception("Nhân viên không tồn tại.");

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
            var todaySchedules = await _context.WorkSchedules
                .Include(s => s.Shift)
                .Where(s => s.EmployeeId == employeeId && s.WorkDate.Date == date.Date)
                .ToListAsync();

            var currentShift = await _context.Shifts.FindAsync(shiftId);
            foreach (var item in todaySchedules)
            {
                if (currentShift.StartTime < item.Shift.EndTime && currentShift.EndTime > item.Shift.StartTime)
                {
                    throw new Exception("Lỗi: Ca đăng ký bị trùng thời gian với ca đã có trong ngày.");
                }
            }

            if (todaySchedules.Count >= employee.Position.MaxShiftPerDay)
            {
                throw new Exception($"Lỗi: Vị trí {employee.Position.Name} chỉ được đăng ký tối đa {employee.Position.MaxShiftPerDay} ca/ngày.");
            }

            // 4. Lưu đăng ký
            var newSchedule = new WorkSchedule
            {
                EmployeeId = employeeId,
                ShiftId = shiftId,
                WorkDate = date,
                Status = "Pending"
            };

            _context.WorkSchedules.Add(newSchedule);
            await _context.SaveChangesAsync();
            return newSchedule;
        }

        public async Task<IEnumerable<WorkSchedule>> GetMySchedulesAsync(int employeeId)
        {
            return await _context.WorkSchedules
                .Include(s => s.Shift)
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.WorkDate)
                .ToListAsync();
        }
    }
}
