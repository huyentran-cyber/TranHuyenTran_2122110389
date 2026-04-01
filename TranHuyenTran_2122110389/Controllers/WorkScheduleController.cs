using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkScheduleController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.WorkSchedules
                .Include(x => x.Employee)
                .Include(x => x.Shift));
        }

        [HttpPost]
        public IActionResult Create(WorkSchedule model)
        {
            _context.WorkSchedules.Add(model);
            _context.SaveChanges();

            return Ok(model);
        }
    }
}