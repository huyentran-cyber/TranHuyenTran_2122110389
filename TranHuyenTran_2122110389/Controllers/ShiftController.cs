using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ShiftController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Shifts);
        }

        [HttpPost]
        public IActionResult Create(Shift model)
        {
            _context.Shifts.Add(model);
            _context.SaveChanges();

            return Ok(model);
        }
    }
}