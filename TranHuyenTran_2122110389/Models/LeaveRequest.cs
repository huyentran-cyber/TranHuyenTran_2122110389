using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranHuyenTran_2122110389.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        [Required]
        public DateTime OffDate { get; set; } // Ngày xin nghỉ

        [Required]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public bool IsEmergency { get; set; } // Đột xuất (nếu nghỉ trước < 2 ngày)

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    }

    public enum LeaveStatus
    {
        Pending,   // Chờ duyệt
        Approved,  // Đồng ý
        Rejected   // Từ chối
    }

}
