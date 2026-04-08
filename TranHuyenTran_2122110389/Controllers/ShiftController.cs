using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.DTOs; 

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ShiftController(AppDbContext context) => _context = context;

        [HttpGet]
        public IActionResult GetAll() => Ok(_context.Shifts.ToList());

        [HttpPost]
        public IActionResult Create(ShiftDTO dto) // Dùng DTO ở đây
        {
            try
            {
                var shift = new Shift
                {
                    Name = dto.Name,
                    // Ép kiểu từ string (Frontend) sang TimeSpan (Backend)
                    StartTime = TimeSpan.Parse(dto.StartTime),
                    EndTime = TimeSpan.Parse(dto.EndTime)
                };

                _context.Shifts.Add(shift);
                _context.SaveChanges();

                return Ok(shift);
            }
            catch (Exception ex)
            {
                return BadRequest("Định dạng thời gian không hợp lệ. Vui lòng dùng HH:mm:ss");
            }
        }
    }
}