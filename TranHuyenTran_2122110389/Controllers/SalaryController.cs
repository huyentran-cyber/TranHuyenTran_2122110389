using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Services.Interfaces;
using TranHuyenTran_2122110389.DTOs;

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SalaryController : ControllerBase
    {
        private readonly ISalaryService _service;
        public SalaryController(ISalaryService service) => _service = service;

        // Manager nhấn nút "Tính lương tháng này"
        [Authorize(Roles = "Manager")]
        [HttpPost("calculate")]
        public async Task<IActionResult> Calculate([FromQuery] int month, [FromQuery] int year)
        {
            // Kiểm tra tính hợp lệ sơ bộ
            if (month < 1 || month > 12) return BadRequest("Tháng không hợp lệ.");

            var result = await _service.CalculateMonthlySalaryAsync(month, year);
            return Ok(new { message = $"Đã tính xong lương tháng {month}/{year}" });
        }

        // Nhân viên tự xem lịch sử lương của mình (Trả về SalaryDTO để thấy tiền phạt)
        [HttpGet("my-history/{employeeId}")]
        public async Task<ActionResult<IEnumerable<SalaryDTO>>> GetMySalary(int employeeId)
        {
            var data = await _service.GetSalaryHistoryAsync(employeeId);
            return Ok(data);
        } 

        // Manager xem báo cáo lương tổng (Trả về SalaryReportDTO để thấy tên nhân viên/vị trí)
        [Authorize(Roles = "Manager")]
        [HttpGet("report")]
        public async Task<ActionResult<IEnumerable<SalaryReportDTO>>> GetReport([FromQuery] int month, [FromQuery] int year)
        {
            var data = await _service.GetMonthlyReportAsync(month, year);
            return Ok(data);
        }
    }
}