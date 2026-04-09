
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IPositionService
    {
        Task<IEnumerable<PositionDTO>> GetAllAsync();
        Task<Position> CreateAsync(PositionDTO dto);
        Task<Position> UpdateAsync(int id, PositionDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
