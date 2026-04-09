using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface ISalaryService
    {
        // Quản lý tính lương cho tất cả nhân viên trong 1 tháng
        Task<bool> CalculateMonthlySalaryAsync(int month, int year);

        // Nhân viên xem lịch sử lương của mình
        Task<IEnumerable<SalaryDTO>> GetSalaryHistoryAsync(int employeeId);

        // Xem bảng lương tổng quát (cho Manager)
        Task<IEnumerable<SalaryReportDTO>> GetMonthlyReportAsync(int month, int year);
    }
}