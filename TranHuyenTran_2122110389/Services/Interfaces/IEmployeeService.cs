using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<Employee>> GetAllAsync(int? month = null, int? year = null, string status = "all");
        Task<Employee> GetByIdAsync(int id);
        Task<Employee> CreateAsync(EmployeeDTO dto);
        Task<Employee> UpdateAsync(int id, EmployeeDTO dto);
        Task<bool> DeleteAsync(int id);
       
    }
}
