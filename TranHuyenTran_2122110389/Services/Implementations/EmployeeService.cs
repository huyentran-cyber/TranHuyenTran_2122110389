using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Helpers;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees.Include(x => x.Position);
        }

        public Employee GetById(int id)
        {
            return _context.Employees.Find(id);
        }

        public Employee Create(EmployeeDTO dto)
        {
            var emp = new Employee
            {
                Name = dto.Name,
                Email = dto.Email,
                Phone = dto.Phone,
                PositionId = dto.PositionId,
                Role = RoleHelper.ParseRole(dto.Role)
            };

            _context.Employees.Add(emp);
            _context.SaveChanges();

            return emp;
        }

        public Employee Update(int id, EmployeeDTO dto)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return null;

            emp.Name = dto.Name;
            emp.Email = dto.Email;
            emp.Phone = dto.Phone;

            _context.SaveChanges();

            return emp;
        }

        public bool Delete(int id)
        {
            var emp = _context.Employees.Find(id);
            if (emp == null) return false;

            _context.Employees.Remove(emp);
            _context.SaveChanges();

            return true;
        }
    }
}
