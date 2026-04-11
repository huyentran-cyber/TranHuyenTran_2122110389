using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Helpers;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly JwtHelper _jwt;

        public AuthService(AppDbContext context, JwtHelper jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        public object Login(LoginDTO dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Password))
                return null;
            // 1. Tìm nhân viên theo Email
            var user = _context.Employees
                .Include(e => e.Position)
                .FirstOrDefault(x => x.Email == dto.Email);

            // 2. Kiểm tra tài khoản tồn tại và xác thực mật khẩu
            if (user == null || !PasswordHelper.VerifyPassword(dto.Password, user.Password))
                return null;

            // 3. KIỂM TRA TRẠNG THÁI HOẠT ĐỘNG
            if (!user.IsActive)
            {
                // Trả về một thông báo lỗi cụ thể thay vì null để Frontend hiển thị cho người dùng
                return new { message = "Tài khoản đã bị khóa hoặc nhân viên đã nghỉ việc." };
            }

            // 4. Nếu mọi thứ ok, tiến hành tạo Token
            var token = _jwt.GenerateToken(user);

            return new
            {
                token,
                user = new
                {
                    user.Id,
                    user.Name,
                    user.Email,
                    user.Role,
                    user.IsActive,
                    Position = user.Position != null ? new
                    {
                        user.Position.Id,
                        user.Position.Name
                    } : null
                }
            };
        }
    }
}