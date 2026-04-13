using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class SalaryService : ISalaryService
    {
        private readonly AppDbContext _context;
        public SalaryService(AppDbContext context) => _context = context;

        public async Task<bool> CalculateMonthlySalaryAsync(int month, int year)
        {
            // 1. Lấy tất cả nhân viên (bao gồm cả người đã nghỉ)
            var employees = await _context.Employees.Include(e => e.Position).ToListAsync();

            foreach (var emp in employees)
            {
                // 2. Lấy dữ liệu chấm công trong tháng/năm được chọn
                var attendances = await _context.Attendances
                    .Where(a => a.EmployeeId == emp.Id && a.CheckIn.Month == month && a.CheckIn.Year == year)
                    .ToListAsync();

                // 3. Lấy dữ liệu lịch làm việc để check nghỉ không phép (Absent)
                var schedules = await _context.WorkSchedules
                    .Where(s => s.EmployeeId == emp.Id && s.WorkDate.Month == month && s.WorkDate.Year == year)
                    .ToListAsync();

                // Chỉ tính toán nếu nhân viên có đi làm hoặc có lịch làm việc
                if (attendances.Any() || schedules.Any())
                {
                    decimal hourlyRate = emp.Position?.HourlyRate ?? 0;

                    // --- A. TÍNH TỔNG GIỜ THỰC TẾ (Để lưu DB & hiển thị báo cáo) ---
                    double actualTotalHours = attendances.Where(a => a.CheckOut != null)
                                                         .Sum(a => (a.CheckOut.Value - a.CheckIn).TotalHours);

                    // --- B. TÍNH TỔNG TIỀN GỐC (Gross) THEO CA (1 ca = 8 tiếng) ---
                    int totalShifts = attendances.Count(a => a.CheckOut != null);
                    decimal totalGrossAmount = totalShifts * 8 * hourlyRate;

                    // --- C. TÍNH PHẠT VI PHẠM (Đi muộn/Về sớm theo giờ thiếu) ---
                    double totalViolationHours = 0;
                    var violationAtts = attendances.Where(a => a.CheckOut != null &&
                                                         !string.IsNullOrEmpty(a.Status) &&
                                                         (a.Status.Contains("Late") || a.Status.Contains("Early")));

                    foreach (var att in violationAtts)
                    {
                        double workDuration = (att.CheckOut.Value - att.CheckIn).TotalHours;
                        if (workDuration < 8) // Nếu làm ít hơn 8 tiếng chuẩn
                        {
                            totalViolationHours += (8 - workDuration); // Cộng dồn số giờ bị thiếu
                        }
                    }
                    // Tiền phạt vi phạm = Số giờ thiếu * Lương 1 giờ
                    decimal penaltyViolation = (decimal)totalViolationHours * hourlyRate;

                    // --- D. TÍNH PHẠT NGHỈ KHÔNG PHÉP (Absent) ---
                    // Chỉ phạt những ngày có trạng thái "Absent" trong lịch làm việc
                    int absentCount = schedules.Count(s => s.Status == "Absent");
                    decimal penaltyAbsent = absentCount * 100000; // Mức phạt 100k/ca nghỉ KP

                    // --- E. TỔNG THỰC NHẬN (TotalAmount) ---
                    // Thực nhận = Lương Gốc (theo ca) - Phạt Giờ Thiếu - Phạt Nghỉ KP
                    decimal totalAmount = totalGrossAmount - penaltyViolation - penaltyAbsent;

                    // Nếu phạt nhiều hơn lương thì gán bằng 0, không để lương âm
                    if (totalAmount < 0) totalAmount = 0;

                    // --- F. LƯU HOẶC CẬP NHẬT VÀO DATABASE ---
                    var existingSalary = await _context.Salaries
                        .FirstOrDefaultAsync(s => s.EmployeeId == emp.Id && s.Month == month && s.Year == year);

                    if (existingSalary != null)
                    {
                        existingSalary.TotalHours = (decimal)actualTotalHours;
                        existingSalary.HourlyRateAtTime = hourlyRate;
                        existingSalary.PenaltyViolation = penaltyViolation;
                        existingSalary.PenaltyAbsent = penaltyAbsent;
                        existingSalary.TotalAmount = totalAmount;
                        existingSalary.CalculatedAt = DateTime.Now;
                    }
                    else
                    {
                        _context.Salaries.Add(new Salary
                        {
                            EmployeeId = emp.Id,
                            Month = month,
                            Year = year,
                            TotalHours = (decimal)actualTotalHours,
                            HourlyRateAtTime = hourlyRate,
                            PenaltyViolation = penaltyViolation,
                            PenaltyAbsent = penaltyAbsent,
                            TotalAmount = totalAmount,
                            CalculatedAt = DateTime.Now
                        });
                    }
                }
            }
            // Lưu tất cả thay đổi vào bảng Salaries
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SalaryReportDTO>> GetMonthlyReportAsync(int month, int year)
        {
            var salaries = await _context.Salaries
                .Include(s => s.Employee).ThenInclude(e => e.Position)
                .Where(s => s.Month == month && s.Year == year)
                .ToListAsync();

            var report = new List<SalaryReportDTO>();

            foreach (var s in salaries)
            {
                var atts = await _context.Attendances
                    .Where(a => a.EmployeeId == s.EmployeeId && a.CheckIn.Month == month && a.CheckIn.Year == year)
                    .ToListAsync();

                var violationList = atts
                    .Where(a => !string.IsNullOrEmpty(a.Status) && (a.Status.Contains("Late") || a.Status.Contains("Early")))
                    .Select(a => a.Status)
                    .Distinct()
                    .ToList();

                var hasAbsent = await _context.WorkSchedules
                    .AnyAsync(ws => ws.EmployeeId == s.EmployeeId &&
                                    ws.WorkDate.Month == month &&
                                    ws.WorkDate.Year == year &&
                                    ws.Status == "Absent");

                        if (hasAbsent)
                        {
                            violationList.Add("Nghỉ không phép");
                        }

                report.Add(new SalaryReportDTO
                {
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee?.Name ?? "N/A",
                    PositionName = s.Employee?.Position?.Name ?? "N/A",
                    TotalDays = atts.Select(a => a.CheckIn.Date).Distinct().Count(),
                    TotalShifts = atts.Count,
                    TotalHours = s.TotalHours,
                    HourlyRate = s.HourlyRateAtTime, // Dùng tỉ giá lúc tính lương
                    Violations = violationList.Any() ? string.Join(", ", violationList) : null,
                    TotalSalary = s.TotalAmount,
                    TotalPenalty = s.PenaltyViolation + s.PenaltyAbsent,
                    PenaltyViolation = s.PenaltyViolation,
                    PenaltyAbsent = s.PenaltyAbsent,
                    Month = s.Month,
                    Year = s.Year
                });
            }

            return report;
        }
        // Hàm này giúp nhân viên xem lại lịch sử lương các tháng trước của chính họ
        public async Task<IEnumerable<SalaryDTO>> GetSalaryHistoryAsync(int employeeId)
        {
            var salaries = await _context.Salaries
        .Where(s => s.EmployeeId == employeeId)
        .ToListAsync();

            var result = new List<SalaryDTO>();
            foreach (var s in salaries)
            {
                // GIỐNG SALARY MANAGER: Đếm số ngày từ bảng Attendances
                int actualDays = await _context.Attendances
                    .Where(a => a.EmployeeId == employeeId && a.CheckIn.Month == s.Month && a.CheckIn.Year == s.Year)
                    .Select(a => a.CheckIn.Date)
                    .Distinct()
                    .CountAsync();

                result.Add(new SalaryDTO    
                {
                    Id = s.Id,
                    Month = s.Month,
                    Year = s.Year,
                    TotalHours = s.TotalHours,
                    TotalAmount = s.TotalAmount,
                    PenaltyViolation = s.PenaltyViolation,
                    PenaltyAbsent = s.PenaltyAbsent,
                    CalculatedAt = s.CalculatedAt,
                    // Gán giá trị đếm được vào đây
                    TotalDays = actualDays
                });
            }
            return result;
        }
    }
}