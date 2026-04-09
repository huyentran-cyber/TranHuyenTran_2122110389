using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranHuyenTran_2122110389.Models
{
    public class Salary
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }

        public int Month { get; set; }
        public int Year { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalHours { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRateAtTime { get; set; }

        // Phạt đi muộn / về sớm (Ví dụ: 20.000đ/lần)
        [Column(TypeName = "decimal(18,2)")]
        public decimal PenaltyViolation { get; set; }

        // Phạt nghỉ không phép: Trừ thêm 1 ngày lương cơ bản của ca đó
        [Column(TypeName = "decimal(18,2)")]
        public decimal PenaltyAbsent { get; set; }

        //  Tổng tiền = (Lương giờ x Giờ làm) – Phạt vi phạm - Phạt nghỉ KP
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime CalculatedAt { get; set; } = DateTime.Now;
    }
}