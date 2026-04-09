using TranHuyenTran_2122110389.DTOs;
using TranHuyenTran_2122110389.Models;

namespace TranHuyenTran_2122110389.Services.Interfaces
{
    public interface ILeaveRequestService
    {
        Task<LeaveRequest> CreateRequestAsync(LeaveRequestDTO dto);
        Task<IEnumerable<LeaveRequest>> GetByEmployeeAsync(int employeeId);
        Task<IEnumerable<LeaveRequest>> GetPendingRequestsAsync();
        Task<bool> ApproveOrRejectAsync(int requestId, LeaveStatus status);
    }
}
