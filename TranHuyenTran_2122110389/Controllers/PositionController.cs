using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }

        // GET ALL
        [HttpGet]
        public IActionResult GetAll()
        {
            var data = _context.Positions.Select(x => new PositionDTO
            {
                Id = x.Id,
                Name = x.Name,
                HourlyRate = x.HourlyRate,
                MinStaff = x.MinStaff,
                MaxShiftPerDay = x.MaxShiftPerDay
            });

            return Ok(data);
        }

        // CREATE
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Create(PositionDTO dto)
        {
            var position = new Position
            {
                Name = dto.Name,
                HourlyRate = dto.HourlyRate,
                MinStaff = dto.MinStaff,
                MaxShiftPerDay = dto.MaxShiftPerDay
            };

            _context.Positions.Add(position);
            _context.SaveChanges();

            return Ok(position);
        }

        // UPDATE
        [HttpPut("{id}")]
        public IActionResult Update(int id, PositionDTO dto)
        {
            var data = _context.Positions.Find(id);

            if (data == null)
                return NotFound();

            data.Name = dto.Name;
            data.HourlyRate = dto.HourlyRate;
            data.MinStaff = dto.MinStaff;
            data.MaxShiftPerDay = dto.MaxShiftPerDay;

            _context.SaveChanges();

            return Ok(data);
        }

        // DELETE
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var data = _context.Positions.Find(id);

            if (data == null)
                return NotFound();

            _context.Positions.Remove(data);
            _context.SaveChanges();

            return Ok("Deleted successfully");
        }
    }
}