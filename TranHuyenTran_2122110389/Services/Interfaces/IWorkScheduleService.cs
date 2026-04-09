using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IWorkScheduleService
    {
        Task<WorkSchedule> RegisterAsync(int employeeId, int shiftId, DateTime date);
        Task<IEnumerable<WorkSchedule>> GetMySchedulesAsync(int employeeId);
    }
}
