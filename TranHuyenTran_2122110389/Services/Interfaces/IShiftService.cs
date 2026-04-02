using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IShiftService
    {
        IEnumerable<Shift> GetAll();
        Shift Create(Shift model);
    }
}
