using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var result = _authService.Login(dto);

            if (result == null)
                return Unauthorized(new { message = "Sai email hoặc mật khẩu" });

            // Kiểm tra nếu result là object chứa message lỗi(tài khoản bị khóa)
            var resultType = result.GetType();
            if (resultType.GetProperty("message") != null && resultType.GetProperty("token") == null)
            {
                return StatusCode(403, result); // 403 Forbidden cho tài khoản bị khóa
            }

            return Ok(result);
        }
    }
}