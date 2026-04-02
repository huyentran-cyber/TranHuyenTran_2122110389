using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IAttendanceService
    {
        IEnumerable<Attendance> GetAll();
        Attendance CheckIn(int employeeId);
        Attendance CheckOut(int employeeId);
    }
}
