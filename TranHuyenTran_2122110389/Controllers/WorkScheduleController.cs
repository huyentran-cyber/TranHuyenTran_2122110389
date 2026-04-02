using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class WorkScheduleController : ControllerBase
    //{
    //    private readonly AppDbContext _context;

    //    public WorkScheduleController(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpGet]
    //    public IActionResult GetAll()
    //    {
    //        return Ok(_context.WorkSchedules
    //            .Include(x => x.Employee)
    //            .Include(x => x.Shift));
    //    }

    //    [HttpPost]
    //    public IActionResult Create(WorkSchedule model)
    //    {
    //        _context.WorkSchedules.Add(model);
    //        _context.SaveChanges();

    //        return Ok(model);
    //    }
    //}
    [Authorize]
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
        public IActionResult Create(WorkScheduleDTO dto)
        {
            var count = _context.WorkSchedules
                .Count(x => x.EmployeeId == dto.EmployeeId && x.WorkDate.Date == dto.WorkDate.Date);

            var position = _context.Employees
                .Include(x => x.Position)
                .FirstOrDefault(x => x.Id == dto.EmployeeId)?.Position;

            if (position == null)
                return BadRequest("Position not found");

            if (count >= position.MaxShiftPerDay)
                return BadRequest("Exceed max shift per day");

            var ws = new WorkSchedule
            {
                EmployeeId = dto.EmployeeId,
                ShiftId = dto.ShiftId,
                WorkDate = dto.WorkDate
            };

            _context.WorkSchedules.Add(ws);
            _context.SaveChanges();

            return Ok(ws);
        }
    }
}