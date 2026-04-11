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

        [HttpGet("available-shifts/{employeeId}")]
        public async Task<IActionResult> GetAvailableShifts(int employeeId)
        {
            try
            {
                var shifts = await _scheduleService.GetAvailableShiftsForEmployeeAsync(employeeId);
                return Ok(shifts);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 1. API lấy danh sách các ca làm đang chờ phê duyệt (Status = 0 hoặc Pending)
        [HttpGet("pending")]
        public async Task<IActionResult> GetPendingSchedules([FromQuery] DateTime? date = null, [FromQuery] int? positionId = null)
        {
            try
            {
                // Bạn cần khai báo hàm này trong IWorkScheduleService
                var pendingList = await _scheduleService.GetPendingSchedulesAsync(date, positionId);

                // Trả về danh sách đã được Map sang DTO để có tên nhân viên, tên ca
                return Ok(pendingList);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. API thực hiện phê duyệt ca làm
        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> ConfirmSchedule(int id)
        {
            try
            {
                // Bạn cần khai báo hàm này trong IWorkScheduleService
                var result = await _scheduleService.ConfirmScheduleAsync(id);

                if (result)
                {
                    return Ok(new { message = "Phê duyệt ca làm thành công!" });
                }
                return BadRequest(new { message = "Không thể phê duyệt ca làm này." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        //từ chối ca làm
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> RejectSchedule(int id)
        {
            try
            {
                var result = await _scheduleService.RejectScheduleAsync(id);
                if (result)
                {
                    return Ok(new { message = "Đã từ chối ca làm việc!" });
                }
                return NotFound("Không tìm thấy ca làm việc.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}