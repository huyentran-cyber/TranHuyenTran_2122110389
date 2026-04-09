using Microsoft.EntityFrameworkCore;
using TranHuyenTran_2122110389.Data;
using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;
using TranHuyenTran_2122110389.Services.Interfaces;

namespace TranHuyenTran_2122110389.Services.Implementations
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly AppDbContext _context;
        public LeaveRequestService(AppDbContext context) => _context = context;

        public async Task<LeaveRequest> CreateRequestAsync(LeaveRequestDTO dto)
        {
            // Logic: Nếu nghỉ trước ít hơn 2 ngày so với hiện tại => Nghỉ đột xuất
            bool isEmergency = (dto.OffDate.Date - DateTime.Now.Date).TotalDays < 2;

            var request = new LeaveRequest
            {
                EmployeeId = dto.EmployeeId,
                OffDate = dto.OffDate,
                Reason = dto.Reason,
                IsEmergency = isEmergency,
                Status = LeaveStatus.Pending
            };

            _context.LeaveRequests.Add(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId)
        {
            return await _context.LeaveRequests
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.OffDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync()
        {
            return await _context.LeaveRequests
                .Include(r => r.Employee)
                .Where(r => r.Status == LeaveStatus.Pending)
                .ToListAsync();
        }

        public async Task<bool> ApproveOrRejectAsync(int requestId, LeaveStatus status)
        {
            var request = await _context.LeaveRequests.FindAsync(requestId);
            if (request == null) return false;

            request.Status = status;


            if (status == LeaveStatus.Approved)
            {
                // Tìm ca làm việc của nhân viên đó vào ngày xin nghỉ
                var schedule = await _context.WorkSchedules
                    .FirstOrDefaultAsync(s => s.EmployeeId == request.EmployeeId
                                         && s.WorkDate.Date == request.OffDate.Date);

                if (schedule != null)
                {
                    // Cập nhật trạng thái lịch thành OnLeave (Nghỉ có phép)
                    // Việc này giúp nhân viên không bị đánh dấu là Absent (Nghỉ không phép)
                    schedule.Status = "OnLeave";
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}