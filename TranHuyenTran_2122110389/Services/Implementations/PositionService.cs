using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class PositionService : IPositionService
    {
        private readonly AppDbContext _context;

        public PositionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PositionDTO>> GetAllAsync()
        {
            return await _context.Positions
                .Select(x => new PositionDTO
                {
                    Id = x.Id,
                    Name = x.Name,
                    HourlyRate = x.HourlyRate,
                    MinStaff = x.MinStaff,
                    MaxShiftPerDay = x.MaxShiftPerDay
                }).ToListAsync();
        }

        public async Task<Position> CreateAsync(PositionDTO dto)
        {
            var position = new Position
            {
                Name = dto.Name,
                HourlyRate = dto.HourlyRate,
                MinStaff = dto.MinStaff,
                MaxShiftPerDay = dto.MaxShiftPerDay
            };
            _context.Positions.Add(position);
            await _context.SaveChangesAsync();
            return position;
        }

        public async Task<Position> UpdateAsync(int id, PositionDTO dto)
        {
            var data = await _context.Positions.FindAsync(id);
            if (data == null) return null;

            data.Name = dto.Name;
            data.HourlyRate = dto.HourlyRate;
            data.MinStaff = dto.MinStaff;
            data.MaxShiftPerDay = dto.MaxShiftPerDay;

            await _context.SaveChangesAsync();
            return data;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var data = await _context.Positions
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (data == null) return false;

            // NGHIỆP VỤ: Không cho xóa nếu đang có nhân viên thuộc vị trí này
            if (data.Employees != null && data.Employees.Any())
                throw new Exception("Không thể xóa vị trí này vì đang có nhân viên đang làm việc.");

            _context.Positions.Remove(data);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
