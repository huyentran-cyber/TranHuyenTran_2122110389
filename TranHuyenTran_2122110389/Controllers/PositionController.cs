using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PositionController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PositionController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Positions);
        }

        [HttpPost]
        public IActionResult Create(Position model)
        {
            _context.Positions.Add(model);
            _context.SaveChanges();

            return Ok(model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Position model)
        {
            var data = _context.Positions.Find(id);

            if (data == null)
                return NotFound();

            data.Name = model.Name;
            data.HourlyRate = model.HourlyRate;

            _context.SaveChanges();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var data = _context.Positions.Find(id);

            if (data == null)
                return NotFound();

            _context.Positions.Remove(data);
            _context.SaveChanges();

            return Ok();
        }
    }
}