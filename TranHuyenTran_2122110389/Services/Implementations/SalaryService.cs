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
        private string FormatMinutesToHours(double totalMinutes)
        {
            if (totalMinutes <= 0) return null;
            int hours = (int)(totalMinutes / 60);
            int minutes = (int)(totalMinutes % 60);

            if (hours > 0)
            {
                return $"{hours}h {minutes}p";
            }
            return $"{minutes}p";
        }
        public SalaryService(AppDbContext context) => _context = context;

        public async Task<bool> CalculateMonthlySalaryAsync(int month, int year)
        {
            var employees = await _context.Employees.Include(e => e.Position).ToListAsync();

            foreach (var emp in employees)
            {
                var attendances = await _context.Attendances
                    .Where(a => a.EmployeeId == emp.Id && a.CheckIn.Month == month && a.CheckIn.Year == year)
                    .ToListAsync();

                var schedules = await _context.WorkSchedules
                    .Include(s => s.Shift)
                    .Where(s => s.EmployeeId == emp.Id && s.WorkDate.Month == month && s.WorkDate.Year == year)
                    .ToListAsync();

                if (!attendances.Any() && !schedules.Any()) continue;

                decimal hourlyRate = emp.Position?.HourlyRate ?? 0;
                double standardTotalHours = 0;
                double totalLateMinutes = 0;
                double totalEarlyMinutes = 0;
                int lateCount = 0;
                int earlyCount = 0;

                foreach (var att in attendances.Where(a => a.CheckOut != null))
                {
                    if (att.ScheduleId == null) continue;
                    var schedule = schedules.FirstOrDefault(s => s.Id == att.ScheduleId);
                    if (schedule?.Shift == null) continue;

                    // 1. TÍNH GIỜ CHUẨN (Lương gốc dựa trên số giờ quy định của ca)
                    double shiftHours = (schedule.Shift.EndTime - schedule.Shift.StartTime).TotalHours;
                    standardTotalHours += (shiftHours <= 6) ? 6 : 8;

                    // 2. TÍNH CHI TIẾT VI PHẠM
                    DateTime shiftStart = schedule.WorkDate.Date.Add(schedule.Shift.StartTime);
                    DateTime shiftEnd = schedule.WorkDate.Date.Add(schedule.Shift.EndTime);

                    if (att.CheckIn > shiftStart.AddMinutes(1)) // Cho phép sai số 1 phút
                    {
                        totalLateMinutes += (att.CheckIn - shiftStart).TotalMinutes;
                        lateCount++;
                    }

                    if (att.CheckOut.Value < shiftEnd.AddMinutes(-1))
                    {
                        totalEarlyMinutes += (shiftEnd - att.CheckOut.Value).TotalMinutes;
                        earlyCount++;
                    }
                }

                // TÍNH TOÁN TIỀN
                decimal totalGrossAmount = (decimal)standardTotalHours * hourlyRate;

                // Phạt cố định theo lần + Phạt theo phút (làm tròn tiền phạt cho đẹp)
                decimal penaltyByTime = Math.Round((decimal)((totalLateMinutes + totalEarlyMinutes) / 60.0) * hourlyRate);
                decimal penaltyViolation = (lateCount * 20000) + (earlyCount * 20000) + penaltyByTime;

                int absentCount = schedules.Count(s => s.Status == "Absent");
                decimal penaltyAbsent = absentCount * 100000;

                decimal totalAmount = Math.Max(0, totalGrossAmount - penaltyViolation - penaltyAbsent);

                // LƯU DB
                var existingSalary = await _context.Salaries
                    .FirstOrDefaultAsync(s => s.EmployeeId == emp.Id && s.Month == month && s.Year == year);

                if (existingSalary != null)
                {
                    existingSalary.TotalHours = (decimal)standardTotalHours; // Lưu số giờ CHUẨN
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
                        TotalHours = (decimal)standardTotalHours,
                        HourlyRateAtTime = hourlyRate,
                        PenaltyViolation = penaltyViolation,
                        PenaltyAbsent = penaltyAbsent,
                        TotalAmount = totalAmount,
                        CalculatedAt = DateTime.Now
                    });
                }
            }

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
                // Lấy chi tiết chấm công để trích xuất lý do vi phạm cụ thể
                var atts = await _context.Attendances
                    .Include(a => a.WorkSchedule).ThenInclude(ws => ws.Shift)
                    .Where(a => a.EmployeeId == s.EmployeeId && a.CheckIn.Month == month && a.CheckIn.Year == year)
                    .ToListAsync();

                var violationDetails = new List<string>();
                double lateMin = 0;
                double earlyMin = 0;

                foreach (var a in atts.Where(x => x.WorkSchedule?.Shift != null && x.CheckOut != null))
                {
                    DateTime sStart = a.WorkSchedule.WorkDate.Date.Add(a.WorkSchedule.Shift.StartTime);
                    DateTime sEnd = a.WorkSchedule.WorkDate.Date.Add(a.WorkSchedule.Shift.EndTime);

                    if (a.CheckIn > sStart.AddMinutes(1)) lateMin += (a.CheckIn - sStart).TotalMinutes;
                    if (a.CheckOut < sEnd.AddMinutes(-1)) earlyMin += (sEnd - a.CheckOut.Value).TotalMinutes;
                }

                if (lateMin > 0) violationDetails.Add($"Đi muộn ({FormatMinutesToHours(lateMin)})");
                if (earlyMin > 0) violationDetails.Add($"Về sớm ({FormatMinutesToHours(earlyMin)})");

                var hasAbsent = await _context.WorkSchedules
                    .AnyAsync(ws => ws.EmployeeId == s.EmployeeId && ws.WorkDate.Month == month && ws.WorkDate.Year == year && ws.Status == "Absent");

                if (hasAbsent) violationDetails.Add("Nghỉ không phép");

                report.Add(new SalaryReportDTO
                {
                    EmployeeId = s.EmployeeId,
                    EmployeeName = s.Employee?.Name ?? "N/A",
                    PositionName = s.Employee?.Position?.Name ?? "N/A",
                    TotalDays = atts.Select(a => a.CheckIn.Date).Distinct().Count(),
                    TotalShifts = atts.Count,
                    TotalHours = s.TotalHours, // Đây là giờ chuẩn (ví dụ 60.0)
                    HourlyRate = s.HourlyRateAtTime,
                    Violations = violationDetails.Any() ? string.Join(", ", violationDetails) : null,
                    GrossSalary = s.TotalHours * s.HourlyRateAtTime,
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

        public async Task<IEnumerable<SalaryDTO>> GetSalaryHistoryAsync(int employeeId)
        {
            var salaries = await _context.Salaries.Where(s => s.EmployeeId == employeeId).ToListAsync();
            var result = new List<SalaryDTO>();
            foreach (var s in salaries)
            {
                int actualDays = await _context.Attendances
                    .Where(a => a.EmployeeId == employeeId && a.CheckIn.Month == s.Month && a.CheckIn.Year == s.Year)
                    .Select(a => a.CheckIn.Date).Distinct().CountAsync();

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
                    TotalDays = actualDays
                });
            }
            return result;
        }
    }
}