using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AttendanceController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Attendances.Include(x => x.Employee));
        }

        [HttpPost]
        public IActionResult CheckIn(Attendance model)
        {
            _context.Attendances.Add(model);
            _context.SaveChanges();

            return Ok(model);
        }
    }
}