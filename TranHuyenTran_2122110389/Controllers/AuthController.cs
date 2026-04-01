using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register(RegisterDTO dto)
        {
            var employee = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = dto.Password,
                PositionId = dto.PositionId,
                Role = Enum.Parse<Role>(dto.Role ?? "Staff")
            };

            _context.Employees.Add(employee);
            _context.SaveChanges();

            return Ok("Register success");
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {
            var user = _context.Employees
                .FirstOrDefault(x => x.Email == dto.Email && x.Password == dto.Password);

            if (user == null)
                return Unauthorized("Invalid account");

            return Ok(user);
        }
    }
}