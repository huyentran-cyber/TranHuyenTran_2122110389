using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveRequestController : ControllerBase
    {
        private readonly ILeaveRequestService _service;
        public LeaveRequestController(ILeaveRequestService service) => _service = service;

        [HttpPost("submit")]
        public async Task<IActionResult> SubmitRequest([FromBody] LeaveRequestDTO dto)
        {
            try
            {
                var result = await _service.CreateRequestAsync(dto);
                return Ok(new { message = "Gửi yêu cầu thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpGet("my-history/{employeeId}")]
        public async Task<IActionResult> GetHistory(int employeeId)
        {
            return Ok(await _service.GetByEmployeeAsync(employeeId));
        }

        [Authorize(Roles = "Manager")]
        [HttpGet("pending")]
        public async Task<IActionResult> GetAllPending()
        {
            return Ok(await _service.GetPendingRequestsAsync());
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("review/{id}")]
        public async Task<IActionResult> Review(int id, [FromQuery] LeaveStatus status)
        {
            var success = await _service.ApproveOrRejectAsync(id, status);
            if (!success) return NotFound("Không tìm thấy đơn xin nghỉ.");
            return Ok(new { message = $"Đã cập nhật trạng thái đơn thành: {status}" });
        }
    }
}