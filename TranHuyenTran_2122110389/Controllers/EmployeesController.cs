using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Helpers;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //public class EmployeeController : ControllerBase
    //{
    //    private readonly AppDbContext _context;

    //    public EmployeeController(AppDbContext context)
    //    {
    //        _context = context;
    //    }

    //    [HttpGet]
    //    public IActionResult GetAll()
    //    {
    //        return Ok(_context.Employees.Include(x => x.Position));
    //    }

    //    [HttpGet("{id}")]
    //    public IActionResult Get(int id)
    //    {
    //        var data = _context.Employees.Find(id);
    //        return Ok(data);
    //    }

    //    [HttpPost]
    //    public IActionResult Create(Employee model)
    //    {
    //        _context.Employees.Add(model);
    //        _context.SaveChanges();
    //        return Ok(model);
    //    }

    //    [HttpPut("{id}")]
    //    public IActionResult Update(int id, Employee model)
    //    {
    //        var data = _context.Employees.Find(id);

    //        if (data == null)
    //            return NotFound();

    //        data.Name = model.Name;
    //        data.Email = model.Email;
    //        data.Phone = model.Phone;

    //        _context.SaveChanges();

    //        return Ok(data);
    //    }

    //    [HttpDelete("{id}")]
    //    public IActionResult Delete(int id)
    //    {
    //        var data = _context.Employees.Find(id);

    //        if (data == null)
    //            return NotFound();

    //        _context.Employees.Remove(data);
    //        _context.SaveChanges();

    //        return Ok();
    //    }
    //}
    [Authorize]
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

        [HttpPost]
        public IActionResult Create(EmployeeDTO dto)
        {
            var emp = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                Password = dto.Password,
                PositionId = dto.PositionId,
                Role = RoleHelper.ParseRole(dto.Role)
            };

            _context.Employees.Add(emp);
            _context.SaveChanges();

            return Ok(emp);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, EmployeeDTO dto)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();

            emp.Name = dto.Name;
            emp.Email = dto.Email;
            emp.Phone = dto.Phone;

            _context.SaveChanges();

            return Ok(emp);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return NotFound();

            _context.Employees.Remove(emp);
            _context.SaveChanges();

            return Ok();
        }
    }
}