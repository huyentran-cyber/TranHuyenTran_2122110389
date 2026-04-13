using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs; 
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _service;
        public ShiftController(IShiftService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ShiftDTO dto)
        {
            try
            {
                var shift = new Shift
                {
                    Name = dto.Name,
                    // Chuyển string "HH:mm" từ Frontend sang TimeSpan
                    StartTime = TimeSpan.Parse(dto.StartTime),
                    EndTime = TimeSpan.Parse(dto.EndTime),
                    PositionId = dto.PositionId,
                    DeptType = dto.DeptType ?? "All"
                };

                var result = await _service.CreateAsync(shift);
                return Ok(result);
            }
            catch (FormatException)
            {
                return BadRequest("Định dạng thời gian không hợp lệ (HH:mm:ss).");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ShiftDTO dto)
        {
            try
            {
                var shift = new Shift
                {
                    Name = dto.Name,
                    StartTime = TimeSpan.Parse(dto.StartTime),
                    EndTime = TimeSpan.Parse(dto.EndTime),
                    PositionId = dto.PositionId,
                    DeptType = dto.DeptType ?? "All"
                };

                var success = await _service.UpdateAsync(id, shift);
                if (success) return Ok(new { message = "Cập nhật ca thành công" });
                return NotFound("Không tìm thấy ca cần sửa");
            }
            catch (FormatException)
            {
                return BadRequest("Định dạng thời gian không hợp lệ (HH:mm).");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _service.DeleteAsync(id);
            if (success) return Ok(new { message = "Xóa ca thành công" });
            return BadRequest("Không thể xóa ca này!");
        }
    }
}