using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TranHuyenTran_2122110389.Services.Implementations;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAttendances([FromQuery] DateTime date)
        {
            var result = await _service.GetAllByDateAsync(date);
            return Ok(result);
        }
        private int GetUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn()
        {
            try
            {
                var userId = GetUserId();
                // Service sẽ tự tìm ca làm việc (WorkSchedule) hiện tại của nhân viên để Check-in
                var result = await _service.CheckInAsync(userId);
                return Ok(new { message = "Check-in thành công", data = result });
            }
            catch (Exception ex)
            {
                // Trả về lỗi nghiệp vụ (Vd: "Bạn không có lịch làm việc trong ca này")
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("checkout")]
        public async Task<IActionResult> CheckOut()
        {
            try
            {
                var userId = GetUserId();
                var result = await _service.CheckOutAsync(userId);
                return Ok(new { message = "Check-out thành công", data = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}