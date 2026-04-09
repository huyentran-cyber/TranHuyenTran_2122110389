using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TranHuyenTran_2122110389.Models
{
    public class Attendance
    {
        [Key]
        public int Id { get; set; }

        public int ScheduleId { get; set; }
        [ForeignKey("ScheduleId")]
        public virtual WorkSchedule? WorkSchedule { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        public DateTime? CheckOut { get; set; }

        public bool IsLate { get; set; } // Tự động tính nếu > 5 phút

        public bool IsEarly { get; set; } // Tự động tính nếu về sớm

        public string? Note { get; set; }
        public string? Status { get; set; }
    }
}   