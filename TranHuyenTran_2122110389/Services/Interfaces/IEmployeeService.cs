using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetAll();
        Employee GetById(int id);
        Employee Create(EmployeeDTO dto);
        Employee Update(int id, EmployeeDTO dto);
        bool Delete(int id);
    }
}
