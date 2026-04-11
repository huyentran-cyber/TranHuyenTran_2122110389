using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Helpers;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        // Chỉ lấy nhân viên đang hoạt động (IsActive = true)
        public async Task<IEnumerable<Employee>> GetAllAsync(int? month = null, int? year = null, string status = "all")
        {
            var query = _context.Employees
            .Include(x => x.Position)
            .Include(x => x.WorkSchedules)
                .ThenInclude(ws => ws.Shift)
        // Sử dụng Filtered Include để chỉ lấy điểm danh đúng tháng/năm yêu cầu
        .Include(x => x.Attendances.Where(a =>
            (!month.HasValue || a.CheckIn.Month == month) &&
            (!year.HasValue || a.CheckIn.Year == year)))
        .AsQueryable();

            // Logic lọc theo trạng thái (status từ frontend gửi lên: all, active, inactive)
            if (status == "active")
            {
                query = query.Where(x => x.IsActive == true);
            }
            else if (status == "inactive")
            {
                query = query.Where(x => x.IsActive == false);
            }
            // Nếu status == "all", không lọc IsActive để hiển thị cả người cũ và mới

            return await query.ToListAsync();
        }

        public async Task<Employee> GetByIdAsync(int id)
        {
            return await _context.Employees
                .Include(x => x.Position)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Employee> CreateAsync(EmployeeDTO dto)
        {
            // 1. Kiểm tra Email đã tồn tại chưa
            var exists = await _context.Employees.AnyAsync(e => e.Email == dto.Email);
            if (exists) throw new Exception("Email này đã được sử dụng.");

            var emp = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PositionId = dto.PositionId,
                // PHẢI CÓ: Mã hóa mật khẩu trước khi lưu (Giả sử bạn có PasswordHelper)
                Password = PasswordHelper.HashPassword(dto.Password),
                Role = RoleHelper.ParseRole(dto.Role),
                IsActive = true
            };

            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();

            return emp;
        }

        public async Task<Employee> UpdateAsync(int id, EmployeeDTO dto)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return null;

            emp.Name = dto.Name;
            emp.Email = dto.Email;
            emp.Phone = dto.Phone;
            emp.PositionId = dto.PositionId;
            emp.IsActive = dto.IsActive;


            // Nếu người dùng nhập mật khẩu mới thì mới Hash lại
            if (!string.IsNullOrEmpty(dto.Password))
            {
                emp.Password = PasswordHelper.HashPassword(dto.Password);
            }

            await _context.SaveChangesAsync();
            return emp;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return false;

            emp.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
