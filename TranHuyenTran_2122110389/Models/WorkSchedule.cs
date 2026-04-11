using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace TranHuyenTran_2122110389.Models
{
    public class WorkSchedule
    {
        [Key]
        public int Id { get; set; }

        public int EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        public int ShiftId { get; set; }
        [ForeignKey("ShiftId")]
        public Shift? Shift { get; set; }

        [Required]
        public DateTime WorkDate { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";
        [JsonIgnore]
        public virtual ICollection<Attendance>? Attendances { get; set; }
    }
}