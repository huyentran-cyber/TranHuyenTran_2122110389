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
            var employees = await _context.Employees.Include(e => e.Position).ToListAsync();

            foreach (var emp in employees)
            {
                if (emp.Position == null) continue;

                // 1. Tính giờ làm thực tế & Phạt vi phạm (Đi muộn/Về sớm)
                var attendances = await _context.Attendances
                    .Where(a => a.EmployeeId == emp.Id && a.CheckIn.Month == month && a.CheckIn.Year == year && a.CheckOut != null)
                    .ToListAsync();

                decimal totalHours = 0;
                decimal penaltyViolation = attendances.Count(a => a.IsLate || a.IsEarly) * 20000; // Phạt 20k/lần

                foreach (var att in attendances)
                {
                    totalHours += (decimal)(att.CheckOut.Value - att.CheckIn).TotalHours;
                }

                // 2. Tính phạt Nghỉ không phép (Absent)
                var absentSchedules = await _context.WorkSchedules
                    .Include(s => s.Shift)
                    .Where(s => s.EmployeeId == emp.Id && s.WorkDate.Month == month && s.WorkDate.Year == year && s.Status == "Absent")
                    .ToListAsync();

                decimal penaltyAbsent = 0;
                foreach (var s in absentSchedules)
                {
                    decimal shiftHours = (decimal)(s.Shift.EndTime - s.Shift.StartTime).TotalHours;
                    penaltyAbsent += (shiftHours * emp.Position.HourlyRate); // Phạt thêm 1 ngày lương cơ bản ca đó
                }

                // 3. Tổng lương thực nhận
                decimal totalAmount = (totalHours * emp.Position.HourlyRate) - penaltyViolation - penaltyAbsent;

                // 4. Lưu dữ liệu
                var existing = await _context.Salaries.FirstOrDefaultAsync(s => s.EmployeeId == emp.Id && s.Month == month && s.Year == year);
                if (existing != null)
                {
                    existing.TotalHours = totalHours;
                    existing.HourlyRateAtTime = emp.Position.HourlyRate;
                    existing.PenaltyViolation = penaltyViolation;
                    existing.PenaltyAbsent = penaltyAbsent;
                    existing.TotalAmount = totalAmount;
                    existing.CalculatedAt = DateTime.Now;
                }
                else
                {
                    _context.Salaries.Add(new Salary
                    {
                        EmployeeId = emp.Id,
                        Month = month,
                        Year = year,
                        TotalHours = totalHours,
                        HourlyRateAtTime = emp.Position.HourlyRate,
                        PenaltyViolation = penaltyViolation,
                        PenaltyAbsent = penaltyAbsent,
                        TotalAmount = totalAmount
                    });
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<SalaryDTO>> GetSalaryHistoryAsync(int employeeId)
        {
            return await _context.Salaries
                .Where(s => s.EmployeeId == employeeId)
                .OrderByDescending(s => s.Year).ThenByDescending(s => s.Month)
                .Select(s => new SalaryDTO
                {
                    Id = s.Id,
                    Month = s.Month,
                    Year = s.Year,
                    TotalHours = s.TotalHours,
                    HourlyRate = s.HourlyRateAtTime,
                    PenaltyViolation = s.PenaltyViolation,
                    PenaltyAbsent = s.PenaltyAbsent,
                    TotalAmount = s.TotalAmount,
                    CalculatedAt = s.CalculatedAt
                }).ToListAsync();
        }

        public async Task<IEnumerable<SalaryReportDTO>> GetMonthlyReportAsync(int month, int year)
        {
            return await _context.Salaries
                .Include(s => s.Employee).ThenInclude(e => e.Position)
                .Where(s => s.Month == month && s.Year == year)
                .Select(s => new SalaryReportDTO
                {
                    EmployeeId = s.EmployeeId,
                    FullName = s.Employee.Name,
                    PositionName = s.Employee.Position.Name,
                    Month = s.Month,
                    Year = s.Year,
                    TotalHours = s.TotalHours,
                    TotalAmount = s.TotalAmount
                }).ToListAsync();
        }
    }
}