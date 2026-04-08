using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IShiftService
    {
        IEnumerable<Shift> GetAll();
        Shift Create(Shift model);


        // BỔ SUNG CÁC HÀM SAU ĐỂ HẾT LỖI GẠCH ĐỎ
        int GetShiftCountByDate(int employeeId, DateTime date);
        WorkSchedule Register(WorkScheduleDTO dto);
        IEnumerable<WorkSchedule> GetByEmployeeId(int employeeId);
    }
}
