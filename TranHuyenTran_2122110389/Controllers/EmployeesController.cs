using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeeController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Employees.Include(x => x.Position));
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var data = _context.Employees.Find(id);
            return Ok(data);
        }

        [HttpPost]
        public IActionResult Create(Employee model)
        {
            _context.Employees.Add(model);
            _context.SaveChanges();
            return Ok(model);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, Employee model)
        {
            var data = _context.Employees.Find(id);

            if (data == null)
                return NotFound();

            data.Name = model.Name;
            data.Email = model.Email;
            data.Phone = model.Phone;

            _context.SaveChanges();

            return Ok(data);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var data = _context.Employees.Find(id);

            if (data == null)
                return NotFound();

            _context.Employees.Remove(data);
            _context.SaveChanges();

            return Ok();
        }
    }
}