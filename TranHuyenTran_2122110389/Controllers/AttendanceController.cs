using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
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

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [HttpPost("checkin")]
        public IActionResult CheckIn()
        {
            try
            {
                var userId = GetUserId();
                return Ok(_service.CheckIn(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("checkout")]
        public IActionResult CheckOut()
        {
            try
            {
                var userId = GetUserId();
                return Ok(_service.CheckOut(userId));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}