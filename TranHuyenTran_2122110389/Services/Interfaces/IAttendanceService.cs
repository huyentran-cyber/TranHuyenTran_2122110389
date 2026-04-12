using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDTO>> GetAllByDateAsync(DateTime date);
        Task<AttendanceDTO> CheckInAsync(int employeeId);
        Task<AttendanceDTO> CheckOutAsync(int employeeId);

    }
}
