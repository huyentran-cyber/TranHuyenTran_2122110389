using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IShiftService _shiftService;
        private readonly IEmployeeService _employeeService;

        public WorkScheduleController(IShiftService shiftService, IEmployeeService employeeService)
        {
            _shiftService = shiftService;
            _employeeService = employeeService;
        }

        [HttpPost("register")]
        public IActionResult RegisterShift([FromBody] WorkScheduleDTO dto)
        {
            var employee = _employeeService.GetById(dto.EmployeeId);
            if (employee == null) return NotFound("Nhân viên không tồn tại.");

            // LOGIC NGHIỆP VỤ: Kiểm tra số ca tối đa theo vị trí
            int currentShifts = _shiftService.GetShiftCountByDate(dto.EmployeeId, dto.Date);
            int maxAllowed = employee.Position.MaxShiftPerDay; // Phục vụ: 2, Pha chế: 1

            if (currentShifts >= maxAllowed)
            {
                return BadRequest($"Lỗi: Bộ phận {employee.Position.Name} chỉ được đăng ký tối đa {maxAllowed} ca/ngày.");
            }

            var result = _shiftService.Register(dto);
            return Ok(result);
        }

        [HttpGet("my-schedule/{employeeId}")]
        public IActionResult GetMySchedule(int employeeId)
        {
            return Ok(_shiftService.GetByEmployeeId(employeeId));
        }
    }
}