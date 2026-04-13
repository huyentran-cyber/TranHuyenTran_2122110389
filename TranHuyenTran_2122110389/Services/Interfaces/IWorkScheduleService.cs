using System.Net.NetworkInformation;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.DTOs;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IWorkScheduleService
    {
        Task<WorkSchedule> RegisterAsync(int employeeId, int shiftId, DateTime date);
        Task<IEnumerable<WorkScheduleDTO>> GetMySchedulesAsync(int employeeId, int month, int year);

        Task<IEnumerable<Shift>> GetAvailableShiftsForEmployeeAsync(int employeeId);

        Task<IEnumerable<WorkScheduleDTO>> GetPendingSchedulesAsync(DateTime? date = null, int? positionId = null);
        Task<bool> ConfirmScheduleAsync(int scheduleId);
        Task<bool> RejectScheduleAsync(int scheduleId);
    }
}
