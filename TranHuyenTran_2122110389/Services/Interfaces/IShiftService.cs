using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IShiftService
    {
        Task<IEnumerable<Shift>> GetAllAsync();
        Task<Shift> CreateAsync(Shift shift);
        Task<bool> UpdateAsync(int id, Shift shift);
        Task<Shift> GetByIdAsync(int id);
        Task<bool> DeleteAsync(int id);
    }
}
