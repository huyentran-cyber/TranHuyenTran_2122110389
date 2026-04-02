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
                return Unauthorized("Invalid account");

            return Ok(result);
        }
    }
}