using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IWorkScheduleService _scheduleService;

        public WorkScheduleController(IWorkScheduleService scheduleService, IEmployeeService employeeService)
        {
            _scheduleService = scheduleService;
            _employeeService = employeeService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterShift([FromBody] WorkScheduleDTO dto)
        {
            var employee = await _employeeService.GetByIdAsync(dto.EmployeeId);
            if (employee == null) return NotFound("Nhân viên không tồn tại.");

            try
            {
                // 2. Gọi Service xử lý đăng ký
                // Service này sẽ tự kiểm tra: Position, MaxShiftPerDay, và Trùng giờ ca làm.
                var result = await _scheduleService.RegisterAsync(dto.EmployeeId, dto.ShiftId, dto.WorkDate);

                // 3. Trả về dữ liệu sạch cho Client
                return Ok(new
                {
                    Message = "Đăng ký ca làm thành công",
                    Data = new
                    {
                        result.Id,
                        result.EmployeeId,
                        result.ShiftId,
                        Date = result.WorkDate.ToString("dd/MM/yyyy"),
                        result.Status
                    }
                });
            }
            catch (Exception ex)
            {
                // Mọi vi phạm quy tắc chặn (Validation) sẽ được ném ra và catch tại đây
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-schedule/{employeeId}")]
        public async Task<IActionResult> GetMySchedule(int employeeId)
        {
            var schedules = await _scheduleService.GetMySchedulesAsync(employeeId);
            return Ok(schedules);
        }
    }
}